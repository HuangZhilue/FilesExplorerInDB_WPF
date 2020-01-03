using System.Collections.ObjectModel;
using System.Windows;

namespace FilesExplorerInDB_WPF.Models
{
    public class SettingsWindowModel : BaseModels
    {
        private bool _isLocal;
        private string _isLocalText;
        private string _dbType;
        private string _dbTypeText;
        private string _connectionString;
        private string _fileStorageLocation;
        private Visibility _isVisibilityFileStorageLocation;
        public ObservableCollection<string> DBTypeItem { get; } = new ObservableCollection<string>();

        public bool IsLocal
        {
            get => _isLocal;
            set
            {
                _isLocal = value;
                OnPropertyChanged(nameof(IsLocal));
            }
        }

        public string IsLocalText
        {
            get => _isLocalText;
            set
            {
                _isLocalText = value;
                OnPropertyChanged(nameof(IsLocalText));
            }
        }

        public string DBType
        {
            get => _dbType;
            set
            {
                _dbType = value;
                OnPropertyChanged(nameof(DBType));
            }
        }

        public string DBTypeText
        {
            get => _dbTypeText;
            set
            {
                _dbTypeText = value;
                OnPropertyChanged(nameof(DBTypeText));
            }
        }

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value;
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public string FileStorageLocation
        {
            get => _fileStorageLocation;
            set
            {
                _fileStorageLocation = value;
                OnPropertyChanged(nameof(FileStorageLocation));
            }
        }

        public Visibility IsVisibilityFileStorageLocation
        {
            get => _isVisibilityFileStorageLocation;
            set
            {
                _isVisibilityFileStorageLocation = value;
                OnPropertyChanged(nameof(IsVisibilityFileStorageLocation));
            }
        }

        public static SettingsWindowModel GetInstance { get; } = new SettingsWindowModel();

        private SettingsWindowModel()
        {
            LoadDBTypeItem();
        }

        private void LoadDBTypeItem()
        {
            DBTypeItem.Clear();
            DBTypeItem.Add("MySQL");
            DBTypeItem.Add("Oracle");
            DBTypeItem.Add("SQL Server");
            DBTypeItem.Add("MongoDB");
        }
    }
}