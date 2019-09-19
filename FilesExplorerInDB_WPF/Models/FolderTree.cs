using FilesExplorerInDB_EF.EFModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FilesExplorerInDB_WPF.Models
{
    public class FolderTree : BaseModels
    {
        private List<Folders> _folderTreeList = new List<Folders>();

        #region 字段

        #region 公共字段

        public List<Folders> FolderTreeList
        {
            get => _folderTreeList;
            set
            {
                _folderTreeList = value; 
                OnPropertyChanged(nameof(FolderTreeList));
            }
        }

        #endregion

        #region 非公共字段

        private List<Folders> RootFolderList { get; set; } =
            FilesDbManager.LoadFoldersEntites(f => f.FolderId != -1 && f.IsDelete == false).ToList();

        #endregion

        #endregion

        #region 构造函数

        public static FolderTree GetInstance { get; } = new FolderTree();

        private FolderTree()
        {
            if (FolderTreeList == null || FolderTreeList.Count <= 0) 
                FolderTreeList = FilesDbManager.GetFoldersTree(-1, RootFolderList);
        }

        #endregion

        public void RefreshFolderTree()
        {
            RootFolderList = FilesDbManager.LoadFoldersEntites(f => f.FolderId != -1 && f.IsDelete == false).ToList();
            FolderTreeList = FilesDbManager.GetFoldersTree(-1, RootFolderList);
        }
    }
}