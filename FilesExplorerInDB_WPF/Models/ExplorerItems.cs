using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Models.Models;
using System.Collections.Generic;
using static Resources.Properties.Settings.SettingType;

namespace FilesExplorerInDB_WPF.Models
{
    public class ExplorerItems : BaseModels
    {
        private List<ExplorerProperty> _explorerList;
        private Folders _folderNow;
        private int _selectIndex;

        public int SelectIndex
        {
            get => _selectIndex;
            set
            {
                _selectIndex = value;
                OnPropertyChanged(nameof(SelectIndex));
            }
        }

        public List<ExplorerProperty> ExplorerList
        {
            get => _explorerList;
            set
            {
                _explorerList = value;
                OnPropertyChanged(nameof(ExplorerList));
            }
        }

        public Folders FolderNow
        {
            get => _folderNow;
            set
            {
                _folderNow = value;
                OnPropertyChanged(nameof(FolderNow));
            }
        }

        public static ExplorerItems GetInstance { get; } = new ExplorerItems();

        private ExplorerItems()
        {
            if (ExplorerList == null) GetFolder(GetSetting(RootFolderId).ToString());
        }

        public void GetFolder(string folderId)
        {
            ExplorerList = FilesDbManager.SetExplorerItemsList(folderId, out Folders folderNow);
            FolderNow = folderNow;
        }
    }
}