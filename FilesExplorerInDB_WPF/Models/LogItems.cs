using FilesExplorerInDB_Models.Models;
using System.Collections.Generic;

namespace FilesExplorerInDB_WPF.Models
{
    public class LogItems : BaseModels
    {
        private List<LogProperty> _explorerList;
        private int _selectIndex;

        public int SelectIndex
        {
            get => _selectIndex;
            set
            {
                _selectIndex = value;
                OnPropertyChanged(nameof(SelectIndex));
            }
        }

        public List<LogProperty> LogList
        {
            get => _explorerList;
            set
            {
                _explorerList = value;
                OnPropertyChanged(nameof(LogList));
            }
        }

        public static LogItems GetInstance { get; } = new LogItems();

        private LogItems()
        {
            if (LogList == null) GetLog();
        }

        public void GetLog()
        {
            LogList = MonitorManager.GetMessageList();
        }
    }
}