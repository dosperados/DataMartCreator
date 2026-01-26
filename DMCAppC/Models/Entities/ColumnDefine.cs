using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMCApp.Models.Entities
{
    [Table("ColumnDefine", Schema = "Setting")]
    public class ColumnDefine : BaseEntity
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ColumnDefineId { get; set; } // Идентификатор (PK) колонки в определении
        [Required] public int ReferenceTableDefineId { get; set; } // Внешний ключ на TableDefineEntity
        [Required] public int TableDefineId { get; set; } // Внешний ключ на TableDefineEntity источника
        [Required] public int SourceDBId { get; set; } // Источник DB
        [Required][MaxLength(128)] public string SourceDBName { get; set; } // Имя источника DB
        [Required] public int SourceTableId { get; set; }
        [Required][MaxLength(128)] public string SourceTableName { get; set; }
        [Required] public int SourceColumnId { get; set; }
        public string CustomeStatment { get; set; } // Пользовательское выражение для колонки источника
        [Required] public int DestDBId { get; set; } // Назначение DB
        [Required][MaxLength(128)] public string DestDBName { get; set; } // Имя назначения DB
        [Required] public int DestTableId { get; set; }
        [Required][MaxLength(128)] public string DestTableName { get; set; }
        [Required] public int DestColumnId { get; set; }
        [Required][MaxLength(128)] public string DestColumnName { get; set; }
        [Required] public float ColumnSortNo { get; set; } // Порядок сортировки колонки в определении
        [Required] public float KeySortNo { get; set; } // Порядок сортировки ключевой колонки в определении
        [Required][MaxLength(2500)] public string Description { get; set; } // Описание колонки в определении

        // Навигационные свойства
        [ForeignKey("ReferenceTableDefineId")] public virtual TableDefine ReferenceTableDefine { get; set; }
        [ForeignKey("TableDefineId")] public virtual TableDefine TableDefine { get; set; }
        [ForeignKey("SourceDBId")] public virtual Db SourceDb { get; set; }
        [ForeignKey("DestDBId")] public virtual Db DestDb { get; set; }
        [ForeignKey("SourceTableId")] public virtual Table SourceTable { get; set; }
        [ForeignKey("DestTableId")] public virtual Table DestTable { get; set; }
        [ForeignKey("SourceColumnId")] public virtual Column SourceColumn { get; set; }
        [ForeignKey("DestColumnId")] public virtual Column DestColumn { get; set; }

    }
}
