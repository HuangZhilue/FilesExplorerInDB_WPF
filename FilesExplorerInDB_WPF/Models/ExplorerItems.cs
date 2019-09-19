using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Models.Models;
using System.Collections.Generic;

namespace FilesExplorerInDB_WPF.Models
{
    public class ExplorerItems : BaseModels
    {
        private List<ExplorerProperty> _explorerList;
        private Folders _folderNow;
        private int _selectIndex;

        #region 字段

        #region 公共字段

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

        #endregion

        #region 非公共字段
        


        #endregion

        #endregion

        #region 构造函数

        public static ExplorerItems GetInstance { get; } = new ExplorerItems();

        private ExplorerItems()
        {
            if (ExplorerList == null) GetFolder(0);
        }

        #endregion

        public void GetFolder(int folderId)
        {
            ExplorerList = FilesDbManager.SetExplorerItemsList(folderId,out Folders folderNow);
            FolderNow = folderNow;
        }

        public void SetExplorerListItem(ExplorerProperty item)
        {
            _explorerList[SelectIndex] = item;
            ExplorerList = _explorerList;
            //OnPropertyChanged(nameof(ExplorerList));
        }
    }
}