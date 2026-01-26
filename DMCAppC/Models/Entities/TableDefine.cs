namespace DMCApp.Models.Entities
{
    public class TableDefine : BaseEntity
    {
        public int TableDefineId { get; set; }
        public int VersionId { get; set; }
        public int SourceMainDBId { get; set; }
        public string SourceMainDBName { get; set; } = null!;
        public string SourceMainSchemaName { get; set; } = null!;
        public int SourceMainTableId { get; set; }
        public string SourceMainTableName { get; set; } = null!;
        public int DestMainDBId { get; set; }
        public string DestMainDBName { get; set; } = null!;
        public string DestMainSchemaName { get; set; } = null!;
        public int DestMainTableId { get; set; }
        public string DestMainTableName { get; set; } = null!;

        // FK Navigation properties
        public virtual Db SourceMainDB { get; set; } = null!;
        public virtual Db DestMainDB { get; set; } = null!;
        public virtual Table SourceMainTable { get; set; } = null!;
        public virtual Table DestMainTable { get; set; } = null!;

        // Incomming FK Navigation properties

    }
}
