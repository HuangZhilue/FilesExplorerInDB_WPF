using FilesExplorerInDB_Models.Models;
using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class LogExplorerVM : BaseViewModel
    {
        #region 字段

        #region 公共字段

        public LogItems LogItems { get; } = LogItems.GetInstance;
        public ContextMenuModel ContextMenuModel { get; } = ContextMenuModel.GetLogInstance;
        public ICommand CommandOpen { get; }
        public ICommand CommandRefresh { get; }
        public ICommand LoadedContextMenu { get; }
        public ICommand DblClick { get; }
        public ICommand Click { get; }
        public ICommand ClickMouseLeftButtonDown { get; }

        #endregion

        #region 非公共字段

        //private PropertyItemVM PropertyItemVM { get; } = PropertyItemVM.GetLogInstance;
        //private PropertyWindowVM PropertyWindowVM { get; } = PropertyWindowVM.GetLogInstance;
        private List<LogProperty> SelectItem { get; set; } = new List<LogProperty>();

        #endregion

        #endregion

        #region 构造函数

        public static LogExplorerVM GetInstance { get; } = new LogExplorerVM();

        private LogExplorerVM()
        {
            LoadedContextMenu = new DelegateCommand<object>(CheckContextMenu, m => true);
            CommandRefresh = new DelegateCommand(Refresh);
            CommandOpen = new DelegateCommand(OpenLog);
            DblClick = new DelegateCommand(Property);
            Click = new DelegateCommand<object>(GetProperty, IsProperty);
            ClickMouseLeftButtonDown = new DelegateCommand<object>(MouseLeftButtonDown, IsProperty);
            Refresh();
        }

        #endregion

        private void CheckContextMenu(object obj)
        {
            System.Collections.IList items = (System.Collections.IList)obj;
            SelectItem = items.Cast<LogProperty>().ToList();
            ContextMenuModel.SetLogMenuItems(SelectItem);
            Debug.WriteLine("CheckContextMenu");
        }

        private void Refresh()
        {
            LogItems.GetLog();
            Debug.WriteLine("Refresh");
        }

        private void OpenLog()
        {
            Debug.WriteLine("OpenLog");
        }

        private void MouseLeftButtonDown(object parameter)
        {
            GetProperty(parameter);
            LogItems.SelectIndex = -1;
            ContextMenuModel.SetLogMenuItems(null);
            Debug.WriteLine("MouseLeftButtonDown");
        }


        private void GetProperty(object parameter)
        {
            //TODO 完善日志的查看功能
            switch (parameter)
            {
                case LogProperty logProperty:
                    //PropertyItemVM.SetProperty(logProperty);
                    SelectItem = new List<LogProperty> { logProperty };
                    Debug.WriteLine("GetProperty1");
                    break;
                case List<LogProperty> exList:
                    SelectItem = exList;
                    //PropertyItemVM.SetProperty(GetSetting(RootFolderId).ToString());
                    Debug.WriteLine("GetProperty2");
                    break;
            }
            ContextMenuModel.SetLogMenuItems(SelectItem);
        }

        private void Property()
        {
            if (LogItems.SelectIndex >= 0)
            {
                //PropertyWindowVM.SetTrashProperty(SelectItem[0]);
                //bool? result = WindowManager.Show(nameof(PropertyWindow), true);
                //if (result != null && result == true)
                    Refresh();
            }
        }

        private static bool IsProperty(object parameter)
        {
            switch (parameter)
            {
                case LogProperty _:
                case List<LogProperty> _:
                    return true;
                default:
                    return false;
            }
        }
    }
}