using FilesExplorerInDB_EF.EFModels;
using System.Collections.Generic;
using static Resources.Resource;

namespace FilesExplorerInDB_WPF.Models
{
    public class FolderTree : BaseModels
    {
        private List<Folders> _folderTreeList = new List<Folders>();

        public List<Folders> FolderTreeList
        {
            get => _folderTreeList;
            set
            {
                _folderTreeList = value;
                OnPropertyChanged(nameof(FolderTreeList));
            }
        }

        private List<Folders> RootFolderList { get; set; } = new List<Folders>();
        //FilesDbManager.LoadFoldersEntities(f => f.FolderId != App_RootLocalFolderId && f.IsDelete == false);

        public static FolderTree GetInstance { get; } = new FolderTree();

        private FolderTree()
        {
            RefreshFolderTree();
            //if (FolderTreeList == null || FolderTreeList.Count <= 0)
            //    FolderTreeList = FilesDbManager.GetFoldersTree(App_RootLocalFolderId, RootFolderList);
        }

        public void RefreshFolderTree()
        {
            RootFolderList = FilesDbManager
                .LoadFoldersEntities(f => f.FolderId != App_RootLocalFolderId && f.IsDelete == false);
            FolderTreeList = FilesDbManager.GetFoldersTree(App_RootLocalFolderId, RootFolderList);
        }
    }
}