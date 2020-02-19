using FilesExplorerInDB_Models.Models;
using FilesExplorerInDB_WPF.Helper;
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
        public LogWindowModel LogWindowModel { get; } = LogWindowModel.GetInstance;
        public LogToolModel LogToolModel { get; } = LogToolModel.GetInstance;
        public ICommand CommandOpen { get; }
        public ICommand CommandRefresh { get; }
        public ICommand CommandReset { get; }
        public ICommand LoadedContextMenu { get; }
        public ICommand DblClick { get; }
        public ICommand Click { get; }
        public ICommand ClickMouseLeftButtonDown { get; }
        public ICommand CommandClose { get; }

        #endregion

        #region 非公共字段

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
            CommandReset = new DelegateCommand(Reset);
            CommandOpen = new DelegateCommand(OpenLog);
            DblClick = new DelegateCommand(Property);
            Click = new DelegateCommand<object>(GetProperty, IsProperty);
            ClickMouseLeftButtonDown = new DelegateCommand<object>(MouseLeftButtonDown, IsProperty);
            CommandClose = new DelegateCommand(Close);
            Refresh();
            LogItems.SelectIndex = -1;
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

        private void Reset()
        {
            LogToolModel.Reset();
        }

        private void OpenLog()
        {
            SetLogWindows();
            Debug.WriteLine("OpenLog");
        }

        private void Close()
        {
            WindowManager.Remove(nameof(LogWindow));
        }

        private void SetLogWindows()
        {
            if (LogItems.SelectIndex > -1 && SelectItem.Count != 0)
            {
                LogWindowModel.MessageType = SelectItem[0].MessageType;
                LogWindowModel.ObjectName = SelectItem[0].ObjectName;
                LogWindowModel.OperationType = SelectItem[0].OperationType;
                LogWindowModel.Operator = SelectItem[0].Operator;
                LogWindowModel.Time = SelectItem[0].Time;
                LogWindowModel.Message = SelectItem[0].Message;
                WindowManager.Show(nameof(LogWindow), true);
            }
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
                OpenLog();
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