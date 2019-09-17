using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FilesExplorerInDB_EF.EFModels;

namespace FilesExplorerInDB_WPF.Models
{
    public class FolderTree : BaseModels
    {
        #region 字段

        #region 公共字段
        public ObservableCollection<Folders> FolderTreeList { get; } = new ObservableCollection<Folders>();

        #endregion

        #region 非公共字段

        private readonly List<Folders> _rootFolderList =
            FilesDbManager.LoadFoldersEntites(f => f.FolderId != -1 && f.IsDelete == false).ToList();

        #endregion

        #endregion

        #region 构造函数

        public static FolderTree GetInstance { get; } = new FolderTree();

        private FolderTree()
        {
            if (FolderTreeList == null || FolderTreeList.Count <= 0) 
                FolderTreeList = new ObservableCollection<Folders>(FilesDbManager.GetFoldersTree(-1, _rootFolderList));
        }

        #endregion

    }
}