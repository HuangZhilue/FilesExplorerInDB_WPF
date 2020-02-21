using FilesExplorerInDB_Models.Models;
using FilesExplorerInDB_WPF.Helper;
using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Resources.Properties.Settings.SettingType;
using static Resources.Resource;
using MessageBox = System.Windows.MessageBox;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class TrashExplorerVM : BaseViewModel
    {
        #region 字段

        #region 公共字段

        public ExplorerItems ExplorerItems { get; } = ExplorerItems.GetTrashInstance;
        public ContextMenuModel ContextMenuModel { get; } = ContextMenuModel.GetTrashInstance;
        public ICommand DblClick { get; }
        public ICommand Click { get; }
        public ICommand ClickMouseLeftButtonDown { get; }
        public ICommand LoadedContextMenu { get; }
        public ICommand CommandRestore { get; }
        public ICommand CommandRefresh { get; }
        public ICommand CommandDelete { get; }
        public ICommand CommandAllDelete { get; }
        public ICommand CommandProperty { get; }

        #endregion

        #region 非公共字段

        private PropertyItemVM PropertyItemVM { get; } = PropertyItemVM.GetTrashInstance;
        private PropertyWindowVM PropertyWindowVM { get; } = PropertyWindowVM.GetTrashInstance;
        private List<ExplorerProperty> SelectItem { get; set; } = new List<ExplorerProperty>();

        #endregion

        #endregion

        #region 构造函数

        public static TrashExplorerVM GetInstance { get; } = new TrashExplorerVM();

        private TrashExplorerVM()
        {
            DblClick = new DelegateCommand(Property);
            Click = new DelegateCommand<object>(GetProperty, IsProperty);
            ClickMouseLeftButtonDown = new DelegateCommand<object>(MouseLeftButtonDown, IsProperty);
            LoadedContextMenu = new DelegateCommand<object>(CheckContextMenu, m => true);
            CommandDelete = new DelegateCommand(Delete);
            CommandAllDelete = new DelegateCommand(AllDelete);
            CommandRestore = new DelegateCommand(Restore);
            CommandProperty = new DelegateCommand(Property);
            CommandRefresh = new DelegateCommand(Refresh);
            ExplorerItems.GetTrash();
            PropertyItemVM.SetProperty(string.Empty);
            GetProperty(null);
        }

        #endregion

        private void MouseLeftButtonDown(object parameter)
        {
            GetProperty(parameter);
            ExplorerItems.SelectIndex = -1;
            ContextMenuModel.SetTrashTabItem(null);
            Debug.WriteLine("MouseLeftButtonDown");
        }

        private void OpenFolder()
        {
            ExplorerItems.GetTrash();
            Debug.WriteLine("OpenFolder1");

            PropertyItemVM.SetProperty(GetSetting(RootFolderId).ToString());
            ContextMenuModel.SetTrashTabItem(null);
        }

        private void GetProperty(object parameter)
        {
            switch (parameter)
            {
                case ExplorerProperty explorerProperty:
                    PropertyItemVM.SetProperty(explorerProperty);
                    SelectItem = new List<ExplorerProperty> {explorerProperty};
                    Debug.WriteLine("GetProperty1");
                    break;
                case List<ExplorerProperty> exList:
                    SelectItem = exList;
                    PropertyItemVM.SetProperty(GetSetting(RootFolderId).ToString());
                    Debug.WriteLine("GetProperty2");
                    break;
            }
            ContextMenuModel.SetTrashTabItem(SelectItem);
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

        private void CheckContextMenu(object obj)
        {
            System.Collections.IList items = (System.Collections.IList)obj;
            SelectItem = items.Cast<ExplorerProperty>().ToList();
            ContextMenuModel.SetTrashMenuItems(SelectItem);
            Debug.WriteLine("CheckContextMenu");
        }

        private void Restore()
        {
            //TODO Restore
            FilesDbManager.Restore(SelectItem);
            Refresh();
        }

        private void Refresh()
        {
            OpenFolder();
        }

        private void Delete()
        {
            var r = MessageBox.Show(Message_DeleteItem, Caption_Info, MessageBoxButton.YesNo);
            if (r == MessageBoxResult.Yes)
                FilesDbManager.CompleteDelete(SelectItem);
            Refresh();
        }

        private void AllDelete()
        {
            var r = MessageBox.Show(Message_DeleteAllItem, Caption_Info, MessageBoxButton.YesNo);
            if (r == MessageBoxResult.Yes)
                FilesDbManager.CompleteDelete(ExplorerItems.ExplorerList);
            Refresh();
        }

        private void Property()
        {
            if (ExplorerItems.SelectIndex >= 0)
            {
                PropertyWindowVM.SetTrashProperty(SelectItem[0]);
                bool? result = WindowManager.Show(nameof(PropertyWindow), true);
                if (result != null && result == true)
                    Refresh();
            }
        }
    }
}