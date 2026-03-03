namespace DMCApp.Models.Entities
{
    public class ReferenceTableDefine : BaseEntity
    {
        public int ReferenceTableDefineId { get; set; }
        public int TableDefineId { get; set; }
        public int VersionId { get; set; } = 1;
        public int? ParentReferenceTableDefineId { get; set; }

        public string RefTableName { get; set; } = string.Empty;
        public int? RefTableId { get; set; }
        public string? RefTableAliasName { get; set; }

        public string? RefType { get; set; }
        public int? RefLevelNo { get; set; }
        public string? JoinType { get; set; }
        public string? Description { get; set; }
    }
}
