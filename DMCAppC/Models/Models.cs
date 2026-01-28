namespace DMCApp.Models;

public class TableDefinition
{
    public int TableDefineId { get; set; }
    public int VersionId { get; set; }
    public int SourceMainDBId { get; set; }
    public string SourceMainDBName { get; set; } = string.Empty;
    public string SourceMainSchemaName { get; set; } = string.Empty;
    public int SourceMainTableId { get; set; }
    public string SourceMainTableName { get; set; } = string.Empty;
    public int DestMainDBId { get; set; }
    public string DestMainDBName { get; set; } = string.Empty;
    public string DestMainSchemaName { get; set; } = string.Empty;
    public int DestMainTableId { get; set; }
    public string DestMainTableName { get; set; } = string.Empty;
    public DateTime ModifiedAt { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public byte Deleted { get; set; }
}

public class ColumnDefinitionViewModel
{
    // This model is used in the Wizard Step 2 grid
    // It maps to GetSourceColumn SQL result + some extra properties

    public string SourceColumnName { get; set; } = string.Empty;
    public string DestColumnName { get; set; } = string.Empty;
    public double ColumnSortNo { get; set; }
    public int SourceColumnId { get; set; } // column_id
    public string DestSchemaName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public bool IsNullable { get; set; }
    public string CollationName { get; set; }
    public string DefaultValue { get; set; }
    public int KeySortNo { get; set; }
    public string SourceKeyColumn { get; set; }
    public string RefTableName { get; set; }

    // Logic properties
    public bool IsOptionField { get; set; }
    public bool AddToDest { get; set; } = true;
    public string FieldType { get; set; } = "Main"; // Main, OptionId, etc.
    public bool IsBlacklisted { get; set; }
    public bool IsNewVirtual { get; set; } // For AOL rows
}

public class ActiveOptionList
{
    public string TableNameDWH { get; set; }
    public string FieldNameDWH { get; set; }
    public string RefTableName { get; set; }
    public string RefColumnName { get; set; }
    public long ColumnSortNo { get; set; }
    public string OptionId { get; set; }
    public int TableNo { get; set; }
    public int FieldNo { get; set; }
}

public class ReferenceTableDefine
{
    public int ReferenceTableDefineId { get; set; }
    public int TableDefineId { get; set; }
    public string RefTableName { get; set; } = string.Empty;
    public string RefType { get; set; } = string.Empty; // Main, OptionList, RefID
    public string JoinType { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
}

public class ReferenceColumnDefine
{
    public int ReferenceColumnDefineId { get; set; }
    public int TableDefineId { get; set; }
    public string SourceColumnName { get; set; } = string.Empty;
    public string DestColumnName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public string FieldType { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;

    // For Lookup
    public int? ReferenceTableId { get; set; }
}
