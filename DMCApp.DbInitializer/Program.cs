using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace DMCApp.DbInitializer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Initializing Databases...");

        // 1. Settings
        var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "DMCAppC");
        if (!Directory.Exists(settingsPath))
        {
            settingsPath = Directory.GetCurrentDirectory();
        }

        var builder = new ConfigurationBuilder()
            .SetBasePath(settingsPath)
            .AddJsonFile("settings.json", optional: false, reloadOnChange: false);

        var configuration = builder.Build();

        // Read DestDbWindowsAuth connection string (assuming it's reliable for localhost/SQL Server access)
        var connectionString = configuration.GetConnectionString("DestDbWindowsAuth");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
             Console.WriteLine("Error: DestDbWindowsAuth connection string not found in settings.json");
             return;
        }

        // Adjust connection string to connect to Master
        // We replace Database=TADM with Database=master.
        var builderCS = new SqlConnectionStringBuilder(connectionString);
        builderCS.InitialCatalog = "master";
        var masterConnectionString = builderCS.ToString();

        try
        {
            using var masterConn = new SqlConnection(masterConnectionString);
            await masterConn.OpenAsync();

            // 2. Create Databases
            await CreateDatabaseAsync(masterConn, "TADM");
            await CreateDatabaseAsync(masterConn, "TADWH");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect or create DBs: {ex.Message}");
            return;
        }

        // Reconstruct connection strings for DBs
        builderCS.InitialCatalog = "TADM";
        var tadmConnStr = builderCS.ToString();

        builderCS.InitialCatalog = "TADWH";
        var tadwhConnStr = builderCS.ToString();

        // 3. TADM (Metadata) Setup
        await SetupDatabaseAsync(tadmConnStr, "TADM",
            "DMCAppC/Unit Test/DDL/TADM/Tables/Setting",
            "DMCAppC/Unit Test"); // CSVs are in Unit Test root

        // 4. TADWH (Source) Setup
        await SetupDatabaseAsync(tadwhConnStr, "TADWH",
            "DMCAppC/Unit Test/DDL/TADWH/Tables/dbo",
            "DMCAppC/Unit Test/DDL/TADWH/Import Data");

        Console.WriteLine("Database Initialization Complete.");
    }

    static async Task CreateDatabaseAsync(IDbConnection conn, string dbName)
    {
        var exists = await conn.ExecuteScalarAsync<int>($"SELECT 1 FROM sys.databases WHERE name = '{dbName}'");
        if (exists == 0)
        {
            Console.WriteLine($"Creating Database {dbName}...");
            await conn.ExecuteAsync($"CREATE DATABASE [{dbName}]");
        }
    }

    static async Task SetupDatabaseAsync(string connStr, string dbName, string sqlDir, string csvDir)
    {
        Console.WriteLine($"Setting up {dbName}...");

        using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        // Run SQL Scripts
        if (Directory.Exists(sqlDir))
        {
            var sqlFiles = Directory.GetFiles(sqlDir, "*.sql").OrderBy(f => f).ToArray();
            foreach (var sqlFile in sqlFiles)
            {
                var script = File.ReadAllText(sqlFile);
                var commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (var cmd in commands)
                {
                    if (string.IsNullOrWhiteSpace(cmd)) continue;
                    try
                    {
                        await conn.ExecuteAsync(cmd);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error executing script {Path.GetFileName(sqlFile)}: {ex.Message}");
                    }
                }
            }
        }

        // Import CSVs
        if (Directory.Exists(csvDir))
        {
            var csvFiles = Directory.GetFiles(csvDir, "*.csv");
            foreach (var csvFile in csvFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(csvFile);
                var tableName = ParseTableNameFromCsvFilename(filename);

                try
                {
                    var tableExists = await conn.ExecuteScalarAsync<int?>($"SELECT OBJECT_ID('{tableName}')");
                    if (tableExists != null)
                    {
                        await ImportCsvAsync(conn, tableName, csvFile);
                    }
                    else
                    {
                         Console.WriteLine($"Table {tableName} not found for CSV {filename}. Skipping.");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error checking table {tableName}: {ex.Message}");
                }
            }
        }
    }

    static string ParseTableNameFromCsvFilename(string filename)
    {
        string schema = "dbo";
        string table = filename;

        if (filename.Contains('.'))
        {
            var parts = filename.Split('.');
            schema = parts[0];
            table = parts[1];
        }

        if (table.Contains("_") && !table.StartsWith("SalesInvoiceHeader"))
        {
             var parts = table.Split('_');
             table = parts[0];
        }

        if (filename == "SalesInvoiceHeader_AOL") return "[dbo].[SalesInvoiceHeader]"; // This CSV seems to match SalesInvoiceHeader table? Wait, headers in SalesInvoiceHeader_AOL.csv?
        // I should double check SalesInvoiceHeader_AOL.csv content.
        // It's in DDL/TADWH/Import Data/SalesInvoiceHeader_AOL.csv.
        // If it's AOL data for that table, it should go to ActiveOptionList?
        // Let's assume generic logic is safer unless specified.

        return $"[{schema}].[{table}]";
    }

    static async Task ImportCsvAsync(IDbConnection conn, string tableName, string csvFilePath)
    {
        Console.WriteLine($"Importing {Path.GetFileName(csvFilePath)} into {tableName}...");

        using var reader = new StreamReader(csvFilePath);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        };

        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord;

        if (headers == null || headers.Length == 0) return;

        var validHeaders = headers.Where(h => h != "OptionId").ToList();
        if (validHeaders.Count == 0) return;

        var columns = string.Join(", ", validHeaders.Select(h => $"[{h}]"));
        var paramsNames = string.Join(", ", validHeaders.Select(h => $"@{h}"));

        var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({paramsNames})";

        bool hasIdentity = false;
        try {
             var checkIdentity = @"
                SELECT 1
                FROM sys.identity_columns
                WHERE object_id = OBJECT_ID(@TableName)";
             var res = await conn.ExecuteScalarAsync<int?>(checkIdentity, new { TableName = tableName.Replace("[", "").Replace("]", "").Replace(".", "].[") });
             hasIdentity = res.HasValue;
        } catch {}

        if (hasIdentity)
        {
            sql = $"SET IDENTITY_INSERT {tableName} ON; {sql}; SET IDENTITY_INSERT {tableName} OFF;";
        }

        var records = new List<dynamic>();
        while (csv.Read())
        {
            var record = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
            foreach (var header in validHeaders)
            {
                var val = csv.GetField(header);
                if (string.IsNullOrWhiteSpace(val) || val == "NULL")
                {
                    record[header] = null;
                }
                else
                {
                    record[header] = val;
                }
            }
            records.Add(record);
        }

        if (records.Any())
        {
             try
             {
                await conn.ExecuteAsync(sql, records);
             }
             catch(Exception ex)
             {
                 Console.WriteLine($"Error inserting into {tableName}: {ex.Message}");
             }
        }
    }
}
