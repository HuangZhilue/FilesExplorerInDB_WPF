using FilesExplorerInDB_Models.Models;
using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using System.Collections.Generic;
using System.Windows.Input;
using FilesExplorerInDB_EF.EFModels;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class ExplorerItemsVM : BaseViewModel
    {
        #region 字段

        #region 公共字段

        public ExplorerItems ExplorerItems { get; } = ExplorerItems.GetInstance;
        public ICommand DblClick { get; }
        public ICommand Click { get; }
        public ICommand ClickPathBack { get; }
        public ICommand ClickPathNext { get; }
        public ICommand ClickPathPrevious { get; }

        #endregion

        #region 非公共字段

        private PropertyItemVM PropertyItemVM { get; } = PropertyItemVM.GetInstance;
        private PathViewVM PathViewVM { get; } = PathViewVM.GetInstance;
        private bool IsPathPrevious { get; set; }

        #endregion

        #endregion

        #region 构造函数

        public static ExplorerItemsVM GetInstance { get; } = new ExplorerItemsVM();

        private ExplorerItemsVM()
        {
            DblClick = new DelegateCommand<object>(OpenFolder, IsValid);
            Click = new DelegateCommand<object>(GetProperty, IsProperty);
            ClickPathBack = new DelegateCommand(Button_PathBack_Click);
            ClickPathNext = new DelegateCommand(Button_PathNext_Click);
            ClickPathPrevious = new DelegateCommand(Button_PathPrevious_Click);
        }

        #endregion

        private void OpenFolder(object parameter)
        {
            switch (parameter)
            {
                case ExplorerProperty explorerProperty:
                    PathViewVM.PathPush(ExplorerItems.FolderNow);
                    ExplorerItems.GetFolder(explorerProperty.Id);
                    PropertyItemVM.SetProperty(ExplorerItems.FolderNow.FolderId);
                    PathViewVM.SetPathString(ExplorerItems.FolderNow.FolderId);
                    break;
                case Folders folders:
                    ExplorerItems.GetFolder(IsPathPrevious ? folders.FolderLocalId : folders.FolderId);
                    IsPathPrevious = false;
                    PropertyItemVM.SetProperty(ExplorerItems.FolderNow.FolderId);
                    PathViewVM.SetPathString(ExplorerItems.FolderNow.FolderId);
                    break;
            }
        }

        private void GetProperty(object parameter)
        {
            switch (parameter)
            {
                case ExplorerProperty explorerProperty:
                    PropertyItemVM.SetProperty(explorerProperty);
                    break;
                case List<ExplorerProperty> _:
                    PropertyItemVM.SetProperty(ExplorerItems.FolderNow.FolderId);
                    break;
            }
        }

        private bool IsProperty(object parameter)
        {
            switch (parameter)
            {
                case ExplorerProperty _:
                case List<ExplorerProperty> _:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsValid(object parameter)
        {
            switch (parameter)
            {
                case ExplorerProperty explorerProperty when explorerProperty.IsFolder:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 历史目录-后退
        /// </summary>
        private void Button_PathBack_Click()
        {
            Folders folders = PathViewVM.PathBack(ExplorerItems.FolderNow);
            OpenFolder(folders);
        }

        /// <summary>
        /// 历史目录-前进
        /// </summary>
        private void Button_PathNext_Click()
        {
            Folders folders = PathViewVM.PathNext(ExplorerItems.FolderNow);
            OpenFolder(folders);
        }

        /// <summary>
        /// 历史目录-上一层
        /// </summary>
        private void Button_PathPrevious_Click()
        {
            Folders folders = PathViewVM.PathPrevious(ExplorerItems.FolderNow);
            IsPathPrevious = true;
            OpenFolder(folders);
        }
    }
}