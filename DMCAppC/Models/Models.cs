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

public partial class ColumnDefinitionViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    // This model is used in the Wizard Step 2 grid
    // It maps to GetSourceColumn SQL result + some extra properties

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _sourceColumnName = string.Empty;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _destColumnName = string.Empty;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private double _columnSortNo;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private int _sourceColumnId;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _destSchemaName = string.Empty;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _dataType = string.Empty;
    partial void OnDataTypeChanged(string value)
    {
        // Add basic reactive smart type behavior similar to Settings defaults
        if (value == "VARBINARY" || value == "nvarchar" || value == "char")
        {
             MaxLength = 1;
             Precision = null;
             Scale = null;
             CollationName = "Latin1_General_CI_AS";
        }
        else if (value == "DATE" || value == "uniqueidentifier")
        {
             MaxLength = null;
             Precision = null;
             Scale = null;
        }
        else if (value == "int")
        {
             MaxLength = null;
             Precision = 10;
             Scale = 0;
        }
    }

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private int? _maxLength;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private int? _precision;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private int? _scale;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private bool _isNullable;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _collationName = string.Empty;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _defaultValue = string.Empty;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private int _keySortNo;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _sourceKeyColumn = string.Empty;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _refTableName = string.Empty;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _destKeyColumn = string.Empty;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private float? _refLevelNo;

    // Logic properties
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private bool _isOptionField;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private bool _addToDest = true;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string _fieldType = "Main"; // Main, OptionId, etc.

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private bool _isBlacklisted;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private bool _isNewVirtual; // For AOL rows
}

public class ActiveOptionList
{
    public string TableNameDWH { get; set; } = string.Empty;
    public string FieldNameDWH { get; set; } = string.Empty;
    public string RefTableName { get; set; } = string.Empty;
    public string RefColumnName { get; set; } = string.Empty;
    public long ColumnSortNo { get; set; }
    public string OptionId { get; set; } = string.Empty;
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
