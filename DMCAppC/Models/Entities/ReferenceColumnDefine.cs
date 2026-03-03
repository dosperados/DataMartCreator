namespace DMCApp.Models.Entities
{
    public class ReferenceColumnDefine : BaseEntity
    {
        public int ReferenceColumnDefineId { get; set; }
        public int TableDefineId { get; set; }
        public int VersionId { get; set; } = 1;

        public int? ReferenceTableDefineId { get; set; }

        public int? SourceColumnId { get; set; }
        public string SourceColumnName { get; set; } = string.Empty;

        public int? DestColumnId { get; set; }
        public string DestColumnName { get; set; } = string.Empty;

        public string DataType { get; set; } = string.Empty;
        public int? MaxLength { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public byte IsNullable { get; set; } = 1;
        public string? CollationName { get; set; }

        public float? ColumnSortNo { get; set; }
        public byte IsOptionField { get; set; } = 0;
        public string? FieldType { get; set; }

        public float? KeySortNo { get; set; }
        public string? SourceKeyColumn { get; set; }
        public string? DestKeyColumn { get; set; }

        public int? AddToDest { get; set; } = 1;

        public string? RefTableName { get; set; }
        public string? RefColumnName { get; set; }
        public float? RefLevelNo { get; set; }
    }
}
