namespace DMCApp.Models.Entities
{
    public class ReferenceTableDefine : BaseEntity
    {
        public int ReferenceTableDefineId { get; set; } // Идентификатор (PK) таблицы в определении
        public int VersionId { get; set; } // Версия определения
        public int? ParentReferenceTableDefineId { get; set; } // Внешний ключ на родительскую ReferenceTableDefine
        public string RefTableName { get; set; } = null!;  // Имя таблицы источника
        public int RefTableId { get; set; }  // Внешний ключ на Table источника
        public string RefTableAliasName { get; set; } = null!; // Псевдоним таблицы источника
        public string? CustomeTableSelect { get; set; } = null!; // Пользовательское выражение для таблицы источника
        public string RefLevelNo { get; set; } = null!; // Порядковый номер ссылки
        public string? RefType { get; set; } = null!; // Тип ссылки
        public string? JoinType { get; set; } = null!; // Тип соединения
        public string? Description { get; set; } // Описание 
        // FK Navigation properties
        public virtual Table RefTable { get; set; } = null!;
        public virtual TableDefine ParenTableDefine { get; set; } = null!;
        
        // Incomming FK Navigation properties

    }
}
