using Dapper;
using DMCApp.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMCApp.Infrastructure;

public interface IMetadataService
{
    Task<IEnumerable<string>> GetSchemasAsync(string connectionString);
    Task<IEnumerable<TableInfo>> GetTablesAsync(string connectionString, string schema, string filter);
    Task<IEnumerable<ColumnDefinitionViewModel>> GetSourceColumnsAsync(string connectionString, string dbName, string schema, string tableName);
    Task<IEnumerable<ActiveOptionList>> GetAolAsync(string connectionString, string sourceMainTable);

    // Config fetches
    Task<IEnumerable<string>> GetDefaultColumnBlackListAsync(string connectionString);
    Task<IEnumerable<ColumnNameChange>> GetDefaultColumnNameChangeAsync(string connectionString);

}

public class TableInfo
{
    public string TableName { get; set; } = string.Empty;
    public string SchemaName { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public DateTime ModifyDate { get; set; }
}

public class ColumnNameChange
{
    public string SourceName { get; set; } = string.Empty;
    public string TargetName { get; set; } = string.Empty;
}

public class SqlMetadataService : IMetadataService
{
    public async Task<IEnumerable<string>> GetSchemasAsync(string connectionString)
    {
        using var conn = new SqlConnection(connectionString);
        var sql = @"
            SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA
            WHERE SCHEMA_NAME NOT IN ('db_owner', 'db_accessadmin', 'db_backupoperator', 'db_datareader','db_datawriter', 'db_ddladmin', 'db_denydatareader', 'db_denydatawriter', 'db_securityadmin', 'INFORMATION_SCHEMA', 'sys')";
        return await conn.QueryAsync<string>(sql);
    }

    public async Task<IEnumerable<TableInfo>> GetTablesAsync(string connectionString, string schema, string filter)
    {
        using var conn = new SqlConnection(connectionString);
        var sql = @"
            SELECT t.[name] as [TableName], s.[name] as [SchemaName], t.[create_date] as [CreateDate], t.[modify_date] as [ModifyDate]
            FROM sys.tables as t
            INNER JOIN sys.[schemas] as s ON t.[schema_id] = s.[schema_id]
            WHERE s.name = @Schema AND t.[name] LIKE @Filter";

        return await conn.QueryAsync<TableInfo>(sql, new { Schema = schema, Filter = $"%{filter}%" });
    }

    public async Task<IEnumerable<ColumnDefinitionViewModel>> GetSourceColumnsAsync(string connectionString, string dbName, string schema, string tableName)
    {
        using var conn = new SqlConnection(connectionString);
        var sql = @"
            SELECT
                'Column' as [Column]
               ,CAST(isc.[ORDINAL_POSITION] as float) as [ColumnSortNo]
               ,sc.[column_id] as [SourceColumnId]
               ,sch.name as [DestSchemaName]
               ,sc.name as [SourceColumnName]
               ,CAST(isc.[DATA_TYPE] as NVARCHAR(128)) as [DataType]
               ,Cast(isc.[CHARACTER_MAXIMUM_LENGTH] as INT) as [MaxLength]
               ,isc.[NUMERIC_PRECISION] as [Precision]
               ,isc.[NUMERIC_SCALE] as [Scale]
               ,case isc.[IS_NULLABLE] when 'NO' then 0 else 1 end as [IsNullable]
               ,sc.[collation_name] as [CollationName]
               ,isc.[Column_Default] as [DefaultValue]
               ,isnull(kcu.[ORDINAL_POSITION],0) as [KeySortNo]
               ,kcu.[COLUMN_NAME] as [SourceKeyColumn]
               ,st.[name]+'_'+sc.name AS [RefTableName]
            FROM sys.tables st
            INNER JOIN sys.columns sc on st.object_id = sc.object_id
            INNER JOIN sys.schemas as sch ON st.[schema_id] = sch.schema_id
            INNER JOIN INFORMATION_SCHEMA.COLUMNS isc ON st.[name] = isc.[TABLE_NAME] AND sch.name = isc.[TABLE_SCHEMA] AND sc.name = isc.[COLUMN_NAME]
            LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
               on kcu.TABLE_CATALOG = isc.TABLE_CATALOG
               and kcu.TABLE_SCHEMA = isc.TABLE_SCHEMA
               and kcu.TABLE_NAME = isc.TABLE_NAME
               and kcu.COLUMN_NAME = isc.COLUMN_NAME
            WHERE st.name = @SourceMainTable AND sch.name = @SourceSchema AND isc.TABLE_CATALOG = @SourceDBName
            ORDER BY isc.[ORDINAL_POSITION];";

        return await conn.QueryAsync<ColumnDefinitionViewModel>(sql, new { SourceMainTable = tableName, SourceSchema = schema, SourceDBName = dbName });
    }

    public async Task<IEnumerable<ActiveOptionList>> GetAolAsync(string connectionString, string sourceMainTable)
    {
        using var conn = new SqlConnection(connectionString);
        var sql = @"
            SELECT aol.[TableNameDWH], aol.[FieldNameDWH], aol.[TableNameDWH]+'_AOL' AS [RefTableName]
            ,aol.[FieldNameDWH] AS [RefColumnName]
            ,DENSE_RANK() OVER ( ORDER BY aol.[TableNo] asc, aol.[FieldNo] ASC) AS [ColumnSortNo]
            ,aol.[OptionId], aol.[TableNo], aol.[FieldNo]
            FROM [TADWH].[dbo].[ActiveOptionList] as aol
            WHERE aol.[TableNameDWH] = @SourceMainTable";

        try
        {
            return await conn.QueryAsync<ActiveOptionList>(sql, new { SourceMainTable = sourceMainTable });
        }
        catch
        {
            // Fallback to current DB if TADWH cross-query isn't available
            sql = sql.Replace("[TADWH].[dbo].", "[dbo].");
            try { return await conn.QueryAsync<ActiveOptionList>(sql, new { SourceMainTable = sourceMainTable }); }
            catch { return new List<ActiveOptionList>(); }
        }
    }

    public async Task<IEnumerable<string>> GetDefaultColumnBlackListAsync(string connectionString)
    {
        using var conn = new SqlConnection(connectionString);
        var sql = "SELECT ColumnName FROM [Setting].[DefaultColumnBlackList]";
        try { return await conn.QueryAsync<string>(sql); }
        catch { return new List<string>(); }
    }

    public async Task<IEnumerable<ColumnNameChange>> GetDefaultColumnNameChangeAsync(string connectionString)
    {
        using var conn = new SqlConnection(connectionString);
        var sql = "SELECT SourceName, TargetName FROM [Setting].[DefaultColumnNameChange]";
        try { return await conn.QueryAsync<ColumnNameChange>(sql); }
        catch { return new List<ColumnNameChange>(); }
    }

}
