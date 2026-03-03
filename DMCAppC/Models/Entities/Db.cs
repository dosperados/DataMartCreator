using System.Collections.Generic;

namespace DMCApp.Models.Entities
{
    public class Db : BaseEntity
    {
        private int _dbId;
        public int DBId
        {
            get => _dbId;
            set => SetProperty(ref _dbId, value);
        }

        private string? _dbName;
        public string? DBName
        {
            get => _dbName;
            set => SetProperty(ref _dbName, value);
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string? _connection;
        public string? Connection
        {
            get => _connection;
            set => SetProperty(ref _connection, value);
        }

        // FK Navigation properties
        public virtual ICollection<Table> Tables { get; set; } = new HashSet<Table>();
    }
}
