namespace FilesExplorerInDB_WPF.Models
{
    public class WindowsModel : BaseModels
    {
        private float _folderTreeWidth = 150f;
        private bool _showExplorer1 = true;
        private bool _showExplorer2;
        private bool _showExplorer3;

        public float FolderTreeWidth
        {
            get => _folderTreeWidth;
            set
            {
                _folderTreeWidth = value;
                OnPropertyChanged(nameof(FolderTreeWidth));
            }
        }

        public bool ShowExplorer1
        {
            get => _showExplorer1;
            set
            {
                _showExplorer1 = value;
                OnPropertyChanged(nameof(ShowExplorer1));
            }
        }

        public bool ShowExplorer2
        {
            get => _showExplorer2;
            set
            {
                _showExplorer2 = value;
                OnPropertyChanged(nameof(ShowExplorer2));
            }
        }

        public bool ShowExplorer3
        {
            get => _showExplorer3;
            set
            {
                _showExplorer3 = value;
                OnPropertyChanged(nameof(ShowExplorer3));
            }
        }

        public static WindowsModel GetInstance { get; } = new WindowsModel();

        private WindowsModel()
        {
        }
    }
}