namespace DMCApp.Models.Entities
{
    public class Column : BaseEntity
    {
        private int _columnId;
        public int ColumnId
        {
            get => _columnId;
            set => SetProperty(ref _columnId, value);
        }

        private int _tableId;
        public int TableId
        {
            get => _tableId;
            set => SetProperty(ref _tableId, value);
        }

        private int _versionId = 1;
        public int VersionId
        {
            get => _versionId;
            set => SetProperty(ref _versionId, value);
        }

        private string _columnName = string.Empty;
        public string ColumnName
        {
            get => _columnName;
            set => SetProperty(ref _columnName, value);
        }

        private string _dataType = string.Empty;
        public string DataType
        {
            get => _dataType;
            set => SetProperty(ref _dataType, value);
        }

        private int? _maxLength;
        public int? MaxLength
        {
            get => _maxLength;
            set => SetProperty(ref _maxLength, value);
        }

        private int? _precision;
        public int? Precision
        {
            get => _precision;
            set => SetProperty(ref _precision, value);
        }

        private int? _scale;
        public int? Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }

        private byte _isNullable = 1;
        public byte IsNullable
        {
            get => _isNullable;
            set => SetProperty(ref _isNullable, value);
        }

        private byte _isPrimaryKey = 0;
        public byte IsPrimaryKey
        {
            get => _isPrimaryKey;
            set => SetProperty(ref _isPrimaryKey, value);
        }

        private string? _collationName;
        public string? CollationName
        {
            get => _collationName;
            set => SetProperty(ref _collationName, value);
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private Table _table = null!;
        public virtual Table Table
        {
            get => _table;
            set
            {
                if (SetProperty(ref _table, value))
                {
                    TableId = value?.TableId ?? 0;
                }
            }
        }
    }
}
