using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMCApp.Models.Entities
{
    public class Table : BaseEntity
    {
        private int _tableId;
        public int TableId
        {
            get => _tableId;
            set => SetProperty(ref _tableId, value);
        }
        private int _dbId;
        public int DBId
        {
            get => _dbId;
            set => SetProperty(ref _dbId, value);
        }


        //public string TableName { get; set; } = null!;
        //public string TableName
        //{
        //    get => _tableName;
        //    set { _tableName = value; OnPropertyChanged(); }
        //}
        private string _tableName;
        public string TableName
        {
            get => _tableName;
            set => SetProperty(ref _tableName, value);
        }
        private string _schemaName = string.Empty;
        public string SchemaName
        {
            get => _schemaName;
            set => SetProperty(ref _schemaName, value);
        }
        private string? _type;
        public string? Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        private string? _mainTableName;
        public string? MainTableName
        {
            get => _mainTableName;
            set => SetProperty(ref _mainTableName, value);
        }
        private string? _mainColumnName;
        public string? MainColumnName
        {
            get => _mainColumnName;
            set => SetProperty(ref _mainColumnName, value);
        }
        private string? _description;
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        // FK Navigation properties
        private Db _db = null!;
        public virtual Db Db
        {
            get => _db;
            set
            {
                if (SetProperty(ref _db, value))
                {
                    DBId = value?.DBId ?? 0;
                }
            }
        }

        // Incomming FK Navigation properties
        public virtual ICollection<Microsoft.EntityFrameworkCore.Metadata.Internal.Column> Columns { get; set; } = new HashSet<Microsoft.EntityFrameworkCore.Metadata.Internal.Column>();
        /*
        public virtual ICollection<TableDefine> TableDefineDestMainTables { get; set; } = new HashSet<TableDefine>();
        public virtual ICollection<TableDefine> TableDefineSourceMainTables { get; set; } = new HashSet<TableDefine>();
        public virtual ICollection<ForeingColumnDefine> ForeingColumnDefines { get; set; } = new HashSet<ForeingColumnDefine>();
        public virtual ICollection<ForeingKeyDefine> ForeingKeyDefines { get; set; } = new HashSet<ForeingKeyDefine>();
        */

    }
}