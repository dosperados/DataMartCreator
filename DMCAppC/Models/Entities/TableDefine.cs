namespace DMCApp.Models.Entities
{
    public class TableDefine : BaseEntity
    {
        public int TableDefineId { get; set; }
        public int VersionId { get; set; } = 1;
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

        public string? Description { get; set; }
    }
}
