namespace DMCApp.Models.Entities
{ 
    public class Column : BaseEntity
    {
        public int ColumnId { get; set; }
        public int TableId { get; set; }
        public string ColumnName { get; set; } = null!;
        public string? Description { get; set; }
        public double ColumnSortNo { get; set; }
        public string DataType { get; set; } = null!;
        public int? MaxLength { get; set; }
        public byte? Precision { get; set; }
        public int? Scale { get; set; }
        public string? CollationName { get; set; }
        public string? DefaultValue { get; set; }
        public double KeySortNo { get; set; }
        public int IsNullable { get; set; }
        public int IsOptionField { get; set; }
        public string? FieldType { get; set; }
        public byte IsLookupField { get; set; }

        // FK Navigation properties
        public virtual Table Table { get; set; } = null!;

        // Incomming FK Navigation properties
        /*
        public virtual ICollection<ColumnDefine> ColumnDefineSourceColumn { get; set; } = new HashSet<ColumnDefine>();
        public virtual ICollection<ColumnDefine> ColumnDefineDestColumn { get; set; } = new HashSet<ColumnDefine>();
        public virtual ICollection<ReferenceColumnDefine> ReferenceColumnDefineRefColumn { get; set; } = new HashSet<ReferenceColumnDefine>();
        public virtual ICollection<ReferenceColumnDefine> ReferenceColumnDefineSourceColumn { get; set; } = new HashSet<ReferenceColumnDefine>();
        public virtual ICollection<ForeingColumnDefine> ForeingColumnDefine { get; set; } = new HashSet<ForeingColumnDefine>();
        public virtual ICollection<ForeingKeyDefine> ForeingKeyDefine { get; set; } = new HashSet<ForeingKeyDefine>();
        public virtual ICollection<IndexDefine> IndexDefine { get; set; } = new HashSet<IndexDefine>();
        */

    }
}
