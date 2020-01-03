using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Models.Models;
using FilesExplorerInDB_WPF.Helper;
using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Resources;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class ExplorerItemsVM : BaseViewModel, IFileDragDropTarget
    {
        #region 字段

        #region 公共字段

        public ExplorerItems ExplorerItems { get; } = ExplorerItems.GetInstance;
        public ContextMenuModel ContextMenuModel { get; } = ContextMenuModel.GetInstance;
        public ICommand DblClick { get; }
        public ICommand Click { get; }
        public ICommand ClickMouseLeftButtonDown { get; }
        public ICommand ClickPathBack { get; }
        public ICommand ClickPathNext { get; }
        public ICommand ClickPathPrevious { get; }
        public ICommand LoadedContextMenu { get; }
        public ICommand CommandOpen { get; }
        public ICommand CommandRefresh { get; }
        public ICommand CommandRefreshAll { get; }
        public ICommand CommandCut { get; }
        public ICommand CommandCopy { get; }
        public ICommand CommandPaste { get; }
        public ICommand CommandCreate { get; }
        public ICommand CommandDelete { get; }
        public ICommand CommandRename { get; }
        public ICommand CommandProperty { get; }
        public ICommand CommandSettings { get; }
        public ICommand CommandLostFocus { get; }
        public ICommand CommandPreviewKeyDown { get; }

        #endregion

        #region 非公共字段

        private PropertyItemVM PropertyItemVM { get; } = PropertyItemVM.GetInstance;
        private FolderTreeVM FolderTreeVM { get; } = FolderTreeVM.GetInstance;
        private PathViewVM PathViewVM { get; } = PathViewVM.GetInstance;
        private PropertyWindowVM PropertyWindowVM { get; } = PropertyWindowVM.GetInstance;
        private bool IsPathPrevious { get; set; }
        private List<ExplorerProperty> SelectItem { get; set; } = new List<ExplorerProperty>();
        private List<ExplorerProperty> SelectItemForPaste { get; set; } = new List<ExplorerProperty>();
        private bool IsCutting { get; set; }
        private bool IsCopying { get; set; }
        private string NameBackup { get; set; }

        #endregion

        #endregion

        #region 构造函数

        public static ExplorerItemsVM GetInstance { get; } = new ExplorerItemsVM();

        private ExplorerItemsVM()
        {
            DblClick = new DelegateCommand<object>(OpenFolder, IsValid);
            Click = new DelegateCommand<object>(GetProperty, IsProperty);
            ClickMouseLeftButtonDown = new DelegateCommand<object>(MouseLeftButtonDown, IsProperty);
            ClickPathBack = new DelegateCommand(Button_PathBack_Click);
            ClickPathNext = new DelegateCommand(Button_PathNext_Click);
            ClickPathPrevious = new DelegateCommand(Button_PathPrevious_Click);
            LoadedContextMenu = new DelegateCommand<object>(CheckContextMenu, m => true);
            CommandCopy = new DelegateCommand(Copy);
            CommandCreate = new DelegateCommand(Create);
            CommandCut = new DelegateCommand(Cut);
            CommandDelete = new DelegateCommand(Delete);
            CommandOpen = new DelegateCommand(Open);
            CommandPaste = new DelegateCommand(Paste);
            CommandProperty = new DelegateCommand(Property);
            CommandSettings = new DelegateCommand(Settings);
            CommandRefresh = new DelegateCommand(Refresh);
            CommandRefreshAll = new DelegateCommand(RefreshAll);
            CommandRename = new DelegateCommand(Rename);
            CommandLostFocus = new DelegateCommand(LostFocus);
            CommandPreviewKeyDown = new DelegateCommand(PreviewKeyDown);
        }

        #endregion

        private void MouseLeftButtonDown(object parameter)
        {
            GetProperty(parameter);
            ExplorerItems.SelectIndex = -1;
        }

        private void OpenFolder(object parameter)
        {
            switch (parameter)
            {
                case ExplorerProperty explorerProperty:
                    PathViewVM.PathPush(ExplorerItems.FolderNow);
                    ExplorerItems.GetFolder(explorerProperty.Id);
                    break;
                case Folders folders:
                    ExplorerItems.GetFolder(IsPathPrevious ? folders.FolderLocalId : folders.FolderId);
                    IsPathPrevious = false;
                    break;
                case int folderLocalId:
                    ExplorerItems.GetFolder(folderLocalId);
                    break;
            }

            PropertyItemVM.SetProperty(ExplorerItems.FolderNow.FolderId);
            PathViewVM.SetPathString(ExplorerItems.FolderNow.FolderId);
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

        private void CheckContextMenu(object obj)
        {
            System.Collections.IList items = (System.Collections.IList) obj;
            SelectItem = items.Cast<ExplorerProperty>().ToList();
            ContextMenuModel.SetMenuItems(SelectItem, IsCopying, IsCutting);
        }

        private void Open()
        {
            if (SelectItem == null || SelectItem.Count < 1) return;
            if (SelectItem[0].IsFolder)
            {
                OpenFolder(SelectItem[0]);
            }
            else
            {
                string path = (GetSetting(SettingType.FileStorageLocation) as string) + SelectItem[0].Id + "." +
                              SelectItem[0].Type;
                if (File.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    MessageBox.Show("文件物理路径错误", Resource.Caption_OpenFileError, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Refresh()
        {
            FolderTreeVM.FolderTree.RefreshFolderTree();
            OpenFolder(ExplorerItems.FolderNow);
        }

        private void RefreshAll()
        {
            FilesDbManager.SetFoldersProperty(0);
            FolderTreeVM.FolderTree.RefreshFolderTree();
        }

        private void Cut()
        {
            SelectItemForPaste = SelectItem;
            IsCopying = false;
            IsCutting = true;
        }

        private void Copy()
        {
            SelectItemForPaste = SelectItem;
            IsCopying = true;
            IsCutting = false;
        }

        private void Paste()
        {
            int folderIdForPaste;
            if (SelectItem != null && SelectItem.Count == 1)
            {
                folderIdForPaste = SelectItemForPaste[0].Id;
            }
            else
            {
                folderIdForPaste = ExplorerItems.FolderNow.FolderId;
            }

            FilesDbManager.Paste(folderIdForPaste, SelectItemForPaste, IsCutting);
            if (IsCutting) MouseLeftButtonDown(SelectItemForPaste);
            Refresh();
            IsCopying = false;
            IsCutting = false;
        }

        private void Create()
        {
            Folders folders = FilesDbManager.CreateFolders(ExplorerItems.FolderNow.FolderId);
            Refresh();
            try
            {
                ExplorerItems.SelectIndex =
                    ExplorerItems.ExplorerList.FindIndex(f => f.Id == folders.FolderId && f.IsFolder);
                Rename();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resource.Caption_Error, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Delete()
        {
            FilesDbManager.SetDeleteState(SelectItem);
            IsCopying = false;
            IsCutting = false;
            Refresh();
        }

        private void Rename()
        {
            ExplorerProperty item = ExplorerItems.ExplorerList[ExplorerItems.SelectIndex];
            NameBackup = item.Name;
            item.BorderThickness = new Thickness(1);
            item.IsReadOnly = false;
            item.Cursor = null;
            item.Focusable = true;
        }

        private void LostFocus()
        {
            ExplorerProperty item = ExplorerItems.ExplorerList.SingleOrDefault(t => t.Focusable && !t.IsReadOnly);
            if (item == null) return;
            item.Name = NameBackup;
            item.BorderThickness = new Thickness(0);
            item.IsReadOnly = true;
            item.Cursor = Cursors.Arrow;
            item.Focusable = false;
        }

        private void PreviewKeyDown()
        {
            NameBackup = ExplorerItems.ExplorerList[ExplorerItems.SelectIndex].Name;
            FilesDbManager.Rename(SelectItem, NameBackup);
            LostFocus();
            Refresh();
            NameBackup = "";
        }

        private void Property()
        {
            if (SelectItem == null || SelectItem.Count < 1)
            {
                var f = ExplorerItems.FolderNow;
                var e = FilesDbManager.SetExplorerItem(f);
                PropertyWindowVM.SetProperty(e);
            }
            else
            {
                PropertyWindowVM.SetProperty(SelectItem[0]);
            }
            bool? result = WindowManager.Show(nameof(PropertyWindow), true);
            if (result != null && result == true)
                Refresh();
        }

        private void Settings()
        {
            WindowManager.Show(nameof(SettingsWindow), true);
        }

        public void OnFileDrop(string[] filePaths)
        {
            //TODO 文件拖放 

            if (filePaths == null) return;
            foreach (var s in filePaths)
            {
                if (File.Exists(s))
                {
                    // 是文件
                    var fi = new FileInfo(s);
                    FilesDbManager.FilesAdd(fi, ExplorerItems.FolderNow.FolderId,
                        GetSetting(SettingType.FileStorageLocation) as string);
                    Refresh();
                }
                else if (Directory.Exists(s))
                {
                    // 是文件夹
                    MessageBox.Show("暂不支持文件夹拖放操作！", Resource.Caption_Info, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 都不是
                    MessageBox.Show("未检测到文件！", Resource.Caption_Info, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}