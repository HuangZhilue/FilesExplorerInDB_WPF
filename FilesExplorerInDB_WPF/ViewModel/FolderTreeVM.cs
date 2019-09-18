using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using System.Windows.Input;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class FolderTreeVM : BaseViewModel
    {
        #region 字段

        #region 公共字段

        public FolderTree FolderTree { get; } = FolderTree.GetInstance;
        public ICommand DblClick { get; }

        #endregion

        #region 非公共字段

        private ExplorerItems ExplorerItems { get; } = ExplorerItems.GetInstance;
        private PropertyItemVM PropertyItemVM { get; } = PropertyItemVM.GetInstance;
        private PathViewVM PathViewVM { get; } = PathViewVM.GetInstance;

        #endregion

        #endregion

        #region 构造函数

        public static FolderTreeVM GetInstance { get; } = new FolderTreeVM();

        private FolderTreeVM()
        {
            DblClick = new DelegateCommand<object>(OpenFolder, IsValid);
        }

        #endregion

        private void OpenFolder(object parameter)
        {
            if (!(parameter is Folders item)) return;
            PathViewVM.PathPrevious(ExplorerItems.FolderNow);
            ExplorerItems.GetFolder(item.FolderId);
            PropertyItemVM.SetProperty(item.FolderId);
            PathViewVM.SetPathString(ExplorerItems.FolderNow.FolderId);
        }

        private bool IsValid(object parameter)
        {
            if (!(parameter is Folders item)) return false;
            return !item.IsDelete;
        }
    }
}