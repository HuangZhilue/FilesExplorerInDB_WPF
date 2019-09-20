using FilesExplorerInDB_EF.EFModels;
using System.Collections.Generic;
using System.Linq;

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

        private List<Folders> RootFolderList { get; set; } =
            FilesDbManager.LoadFoldersEntites(f => f.FolderId != -1 && f.IsDelete == false).ToList();

        public static FolderTree GetInstance { get; } = new FolderTree();

        private FolderTree()
        {
            if (FolderTreeList == null || FolderTreeList.Count <= 0)
                FolderTreeList = FilesDbManager.GetFoldersTree(-1, RootFolderList);
        }

        public void RefreshFolderTree()
        {
            RootFolderList = FilesDbManager.LoadFoldersEntites(f => f.FolderId != -1 && f.IsDelete == false).ToList();
            FolderTreeList = FilesDbManager.GetFoldersTree(-1, RootFolderList);
        }
    }
}