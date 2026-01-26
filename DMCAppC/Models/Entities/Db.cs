using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMCApp.Models.Entities
{
    [Table("DB", Schema = "Setting")]
    public class Db : BaseEntity // Наследование BaseEntity для ModifiedAt, ModifiedBy, Deleted
    {
        public int DBId { get; set; }

        public string? DBName { get; set; }

        public string? Description { get; set; }

        public string? Connection { get; set; } = string.Empty;

        // FK Navigation properties
        public virtual ICollection<Table> Tables { get; set; } = new HashSet<Table>();

        // Incomming FK Navigation properties
        public virtual ICollection<TableDefine> SourceMainDBIds { get; set; } = new HashSet<TableDefine>();
        public virtual ICollection<TableDefine> DestMainDBIds { get; set; } = new HashSet<TableDefine>();
    }
}
