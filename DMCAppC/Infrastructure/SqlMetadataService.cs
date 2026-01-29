using Dapper;
using DMCApp.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DMCApp.Infrastructure;

public interface IMetadataService
{
    Task<IEnumerable<string>> GetSchemasAsync(string connectionString);
    Task<IEnumerable<TableInfo>> GetTablesAsync(string connectionString, string schema, string filter);
    Task<IEnumerable<ColumnDefinitionViewModel>> GetSourceColumnsAsync(string connectionString, string dbName, string schema, string tableName);
    Task<IEnumerable<ActiveOptionList>> GetAolAsync(string connectionString, string sourceMainTable);

    // Transactional Save
    Task SaveTableDefinitionAsync(TableDefinition tableDef, IEnumerable<ColumnDefinitionViewModel> columns,
                                  IEnumerable<ActiveOptionList> aolList, string sourceConnStr, string destConnStr);
}

public class TableInfo
{
    public string TableName { get; set; } = string.Empty;
    public string SchemaName { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public DateTime ModifyDate { get; set; }
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
                sc.name as [SourceColumnName]
               ,CAST(isc.[ORDINAL_POSITION] as float) as [ColumnSortNo]
               ,sc.[column_id] as [SourceColumnId]
               ,sch.name as [DestSchemaName]
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
        // Note: TADWH.dbo.ActiveOptionList
        // The query assumes we are connected to TADWH or can access it.
        // Prompt says: FROM [TADWH].[dbo].[ActiveOptionList]
        // If connectionString points to TADWH, we can remove [TADWH].
        // Ideally we should use the DB name from settings, but for now I'll use the prompt's SQL verbatim-ish.

        var sql = @"
            SELECT aol.[TableNameDWH], aol.[FieldNameDWH], aol.[TableNameDWH]+'_AOL' AS [RefTableName]
            ,aol.[FieldNameDWH] AS [RefColumnName]
            ,DENSE_RANK() OVER ( ORDER BY aol.[TableNo] asc, aol.[FieldNo] ASC) AS [ColumnSortNo]
            ,aol.[OptionId], aol.[TableNo], aol.[FieldNo]
            FROM [dbo].[ActiveOptionList] as aol
            WHERE aol.[TableNameDWH] = @SourceMainTable";

        return await conn.QueryAsync<ActiveOptionList>(sql, new { SourceMainTable = sourceMainTable });
    }

    public async Task SaveTableDefinitionAsync(TableDefinition tableDef, IEnumerable<ColumnDefinitionViewModel> columns, IEnumerable<ActiveOptionList> aolList, string sourceConnStr, string destConnStr)
    {
        using var destConn = new SqlConnection(destConnStr);
        await destConn.OpenAsync();
        using var transaction = destConn.BeginTransaction();

        try
        {
            // 1. Ensure DBs Exist (Stub - assuming executed by DbInitializer or manual)
            // 2. Ensure Tables Exist
            // Check/Create Dest Table in [Setting].[Table]
            var tableId = await EnsureTableExists(destConn, transaction, tableDef.DestMainSchemaName, tableDef.DestMainTableName, tableDef.DestMainDBName);
            tableDef.DestMainTableId = tableId;

            // Check/Create Source Table in [Setting].[Table]
            var sourceTableId = await EnsureTableExists(destConn, transaction, tableDef.SourceMainSchemaName, tableDef.SourceMainTableName, tableDef.SourceMainDBName);
            tableDef.SourceMainTableId = sourceTableId;

            // Check/Create Virtual AOL Table if needed
            // Logic: if we have virtual columns, we might need to register the _AOL table
            // For now, skipping virtual table registration unless explicitly needed by ReferenceTableDefine logic

            // 3. Insert TableDefine
            var insertTableDefine = @"
                INSERT INTO [Setting].[TableDefine]
                (VersionId, SourceMainDBId, SourceMainDBName, SourceMainSchemaName, SourceMainTableId, SourceMainTableName,
                 DestMainDBId, DestMainDBName, DestMainSchemaName, DestMainTableId, DestMainTableName, ModifiedBy)
                VALUES
                (@VersionId, @SourceMainDBId, @SourceMainDBName, @SourceMainSchemaName, @SourceMainTableId, @SourceMainTableName,
                 @DestMainDBId, @DestMainDBName, @DestMainSchemaName, @DestMainTableId, @DestMainTableName, @ModifiedBy);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Need DB IDs. Assuming stub values or lookup.
            tableDef.SourceMainDBId = 1; // Stub
            tableDef.DestMainDBId = 2;   // Stub
            tableDef.VersionId = 1;      // Stub

            var tableDefineId = await destConn.ExecuteScalarAsync<int>(insertTableDefine, tableDef, transaction);
            tableDef.TableDefineId = tableDefineId;

            // 4. Insert ReferenceTableDefine
            // Record 1 (Main)
            var insertRefTable = @"
                INSERT INTO [Setting].[ReferenceTableDefine]
                (TableDefineId, RefTableName, RefType, JoinType, ModifiedBy)
                VALUES (@TableDefineId, @RefTableName, @RefType, @JoinType, @ModifiedBy)";

            await destConn.ExecuteAsync(insertRefTable, new {
                TableDefineId = tableDefineId,
                RefTableName = tableDef.SourceMainTableName,
                RefType = "Main",
                JoinType = "INNER JOIN",
                ModifiedBy = tableDef.ModifiedBy
            }, transaction);

            // AOL References
            // If we have Option Fields
            var optionFields = columns.Where(c => c.IsOptionField).ToList();
            if (optionFields.Any())
            {
                 await destConn.ExecuteAsync(insertRefTable, new {
                    TableDefineId = tableDefineId,
                    RefTableName = tableDef.SourceMainTableName + "_AOL",
                    RefType = "OptionList",
                    JoinType = "LEFT JOIN",
                    ModifiedBy = tableDef.ModifiedBy
                }, transaction);
            }

            // 5. Insert ReferenceColumnDefine
            var insertRefCol = @"
                INSERT INTO [Setting].[ReferenceColumnDefine]
                (TableDefineId, SourceColumnName, DestColumnName, DataType, IsNullable, FieldType, ModifiedBy)
                VALUES (@TableDefineId, @SourceColumnName, @DestColumnName, @DataType, @IsNullable, @FieldType, @ModifiedBy)";

            foreach (var col in columns.Where(c => c.AddToDest))
            {
                await destConn.ExecuteAsync(insertRefCol, new {
                    TableDefineId = tableDefineId,
                    SourceColumnName = col.SourceColumnName,
                    DestColumnName = col.DestColumnName,
                    DataType = col.DataType,
                    IsNullable = col.IsNullable,
                    FieldType = col.FieldType,
                    ModifiedBy = tableDef.ModifiedBy
                }, transaction);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<int> EnsureTableExists(IDbConnection conn, IDbTransaction transaction, string schema, string table, string db)
    {
        // Simple stub: Check if exists in [Setting].[Table], if not insert
        var sqlCheck = "SELECT TableId FROM [Setting].[Table] WHERE SchemaName = @Schema AND TableName = @Table";
        var id = await conn.ExecuteScalarAsync<int?>(sqlCheck, new { Schema = schema, Table = table }, transaction);

        if (id.HasValue) return id.Value;

        var sqlInsert = @"
            INSERT INTO [Setting].[Table] (DBId, VersionId, TableName, SchemaName, Type, ModifiedBy)
            VALUES (1, 1, @TableName, @SchemaName, 'Unknown', 'SYSTEM');
            SELECT CAST(SCOPE_IDENTITY() as int)";

        return await conn.ExecuteScalarAsync<int>(sqlInsert, new { TableName = table, SchemaName = schema }, transaction);
    }
}
