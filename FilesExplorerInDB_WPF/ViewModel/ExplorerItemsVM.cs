using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Models.Models;
using FilesExplorerInDB_WPF.Helper;
using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using static System.String;
using static Resources.Properties.Settings.SettingType;
using static Resources.Resource;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;

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
        public ICommand KeyDownPathCheck { get; }
        public ICommand ClickSearchCancel { get; }
        public ICommand ClickSearchEnter { get; }
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
        public System.Windows.Media.ImageSource SettingImage => FilesDbManager.GetImage(cog_regular_120);
        public System.Windows.Media.ImageSource RefreshAllImage => FilesDbManager.GetImage(revision_regular_120);

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
            KeyDownPathCheck = new DelegateCommand(PathCheck);
            ClickSearchCancel = new DelegateCommand(CancelSearch);
            ClickSearchEnter = new DelegateCommand(SearchExplorer);
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
            PathViewVM.SetPathString(GetSetting(RootFolderId).ToString());
        }

        #endregion

        private void PathCheck()
        {
            var path = PathViewVM.PathViewModel.PathString;
            if (IsNullOrWhiteSpace(path)) return;
            var matches = Regex.Matches(path, @"[^\\\/]+");
            if (matches.Count <= 0) return;
            var lastFolderLocalId = App_RootLocalFolderId;
            foreach (Match match in matches)
            {
                var folderName = match.ToString();
                var id = lastFolderLocalId;
                if (!(FilesDbManager.LoadFoldersEntities(f => f.FolderLocalId == id)
                    .Find(n => n.FolderName == folderName) is Folders folder))
                {
                    MessageBox.Show(Message_PathCheckError, Caption_Error, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                lastFolderLocalId = folder.FolderId;
            }

            OpenFolder(lastFolderLocalId);
        }

        private void MouseLeftButtonDown(object parameter)
        {
            GetProperty(parameter);
            ExplorerItems.SelectIndex = -1;
            LostFocus();
            ContextMenuModel.SetViewTabItems(null, IsCopying, IsCutting);
            Debug.WriteLine("MouseLeftButtonDown");
        }

        private void OpenFolder(object parameter)
        {
            switch (parameter)
            {
                case ExplorerProperty explorerProperty:
                    PathViewVM.PathPush(ExplorerItems.FolderNow);
                    ExplorerItems.GetFolder(explorerProperty.Id);
                    Debug.WriteLine("OpenFolder1");
                    break;
                case Folders folders:
                    ExplorerItems.GetFolder(IsPathPrevious ? folders.FolderLocalId : folders.FolderId);
                    IsPathPrevious = false;
                    Debug.WriteLine("OpenFolder2");
                    break;
                case string folderLocalId:
                    ExplorerItems.GetFolder(folderLocalId);
                    Debug.WriteLine("OpenFolder3");
                    break;
            }
            PropertyItemVM.SetProperty(ExplorerItems.FolderNow.FolderId);
            PathViewVM.SetPathString(ExplorerItems.FolderNow.FolderId);
            ContextMenuModel.SetViewTabItems(null, IsCopying, IsCutting);
        }

        private void GetProperty(object parameter)
        {
            //TODO 在这里开始，做“主页”工具栏
            switch (parameter)
            {
                case ExplorerProperty explorerProperty:
                    PropertyItemVM.SetProperty(explorerProperty);
                    SelectItem = new List<ExplorerProperty> {explorerProperty};
                    Debug.WriteLine("GetProperty1");
                    break;
                case List<ExplorerProperty> exList:
                    SelectItem = exList;
                    PropertyItemVM.SetProperty(ExplorerItems.FolderNow.FolderId);
                    Debug.WriteLine("GetProperty2");
                    break;
            }
            ContextMenuModel.SetViewTabItems(SelectItem, IsCopying, IsCutting);
        }

        private static bool IsProperty(object parameter)
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

        private static bool IsValid(object parameter)
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

        private void CancelSearch()
        {
            Refresh();
            PathViewVM.PathViewModel.SearchString = "";
        }

        private void SearchExplorer()
        {
            ExplorerItems.ExplorerList =
                FilesDbManager.SearchResultList(PathViewVM.PathViewModel.SearchString, ExplorerItems.FolderNow);
        }

        private void CheckContextMenu(object obj)
        {
            System.Collections.IList items = (System.Collections.IList)obj;
            SelectItem = items.Cast<ExplorerProperty>().ToList();
            ContextMenuModel.SetMenuItems(SelectItem, IsCopying, IsCutting);
            Debug.WriteLine("CheckContextMenu");
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
                Files files = FilesDbManager.FilesFind(SelectItem[0].Id);
                if (files != null)
                {
                    var path = files.RealName;
                    if (File.Exists(path))
                    {
                        Process.Start(path);
                    }
                    else
                    {
                        MessageBox.Show("文件物理路径错误", Caption_OpenFileError, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
            FilesDbManager.SetFoldersProperty(GetSetting(RootFolderId).ToString());
            FolderTreeVM.FolderTree.RefreshFolderTree();
        }

        private void Cut()
        {
            SelectItemForPaste = SelectItem;
            IsCopying = false;
            IsCutting = true;
            ContextMenuModel.IsVisiblePaste = Visibility.Visible;
        }

        private void Copy()
        {
            SelectItemForPaste = SelectItem;
            IsCopying = true;
            IsCutting = false;
            ContextMenuModel.IsVisiblePaste = Visibility.Visible;
        }

        private void Paste()
        {
            string folderIdForPaste;
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
                MessageBox.Show(ex.Message, Caption_Error, MessageBoxButton.OK, MessageBoxImage.Information);
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
            item.IsFocused = true;
        }

        private void LostFocus()
        {
            ExplorerProperty item = ExplorerItems.ExplorerList.SingleOrDefault(t => t.Focusable && !t.IsReadOnly);
            if (item == null) return;
            if (ExplorerItems.SelectIndex > -1 && NameBackup != null) PreviewKeyDown();
            item.Name = NameBackup;
            item.BorderThickness = new Thickness(0);
            item.IsReadOnly = true;
            item.Cursor = Cursors.Arrow;
            item.Focusable = false;
            item.IsFocused = false;
        }

        private void PreviewKeyDown()
        {
            NameBackup = ExplorerItems.ExplorerList[ExplorerItems.SelectIndex].Name.TrimStart();
            if (NameBackup.CheckNameIsNullOrWhiteSpace())
            {
                MessageBox.Show(Message_NameCheckIsNullOrWhiteSpace, Caption_Info, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (NameBackup.CheckNameIsNameRegex())
            {
                FilesDbManager.Rename(SelectItem, NameBackup);
            }
            else
            {
                MessageBox.Show(Message_NameCheckError, Caption_Info, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            NameBackup = null;
            LostFocus();
            Refresh();
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
            if (filePaths == null) return;
            foreach (var s in filePaths)
            {
                if (File.Exists(s))
                {
                    // 是文件
                    var fi = new FileInfo(s);
                    FilesDbManager.FilesAdd(fi, ExplorerItems.FolderNow.FolderId,
                        GetSetting(FileStorageLocation) as string);
                    Refresh();
                }
                else if (Directory.Exists(s))
                {
                    // 是文件夹
                    MessageBox.Show("暂不支持文件夹拖放操作！", Caption_Info, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 都不是
                    MessageBox.Show("未检测到文件！", Caption_Info, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}