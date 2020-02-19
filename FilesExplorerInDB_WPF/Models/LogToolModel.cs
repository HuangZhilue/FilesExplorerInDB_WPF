using System;
using System.Collections.ObjectModel;
using static System.String;
using static FilesExplorerInDB_Manager.Implements.MonitorManager;

namespace FilesExplorerInDB_WPF.Models
{
    public class LogToolModel : BaseModels
    {
        private DateTime _timeStart = DateTime.Today;
        private DateTime _timeEnd = DateTime.Now;
        private string _operationType;
        private string _objectName;
        private string _message;
        public ObservableCollection<string> MessageTypeItem { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> OperatorItem { get; } = new ObservableCollection<string>();
        private int _messageTypeItemIndex = 0;
        private int _operatorItemIndex = 0;

        public DateTime TimeStart
        {
            get => _timeStart;
            set
            {
                _timeStart = value;
                OnPropertyChanged(nameof(TimeStart));
            }
        }

        public DateTime TimeEnd
        {
            get => _timeEnd;
            set
            {
                _timeEnd = value;
                OnPropertyChanged(nameof(TimeEnd));
            }
        }

        public string OperationType
        {
            get => _operationType;
            set
            {
                _operationType = value;
                OnPropertyChanged(nameof(OperationType));
            }
        }

        public string ObjectName
        {
            get => _objectName;
            set
            {
                _objectName = value;
                OnPropertyChanged(nameof(ObjectName));
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public static LogToolModel GetInstance { get; } = new LogToolModel();

        public int MessageTypeItemIndex
        {
            get => _messageTypeItemIndex;
            set
            {
                _messageTypeItemIndex = value;
                OnPropertyChanged(nameof(MessageTypeItemIndex));
            }
        }

        public int OperatorItemIndex
        {
            get => _operatorItemIndex;
            set
            {
                _operatorItemIndex = value;
                OnPropertyChanged(nameof(OperatorItemIndex));
            }
        }

        private LogToolModel()
        {
            SetMessageTypeItem();
            SetOperatorItem();
        }

        private void SetMessageTypeItem()
        {
            MessageTypeItem.Clear();
            var i = 0;
            MessageTypeItem.Add("");
            while (true)
            {
                string r = MonitorManager.GetMessageTypeName(i++);
                if (!IsNullOrWhiteSpace(r))
                    MessageTypeItem.Add(r);
                else break;
            }
        }

        private void SetOperatorItem()
        {
            OperatorItem.Clear();
            var i = 0;
            OperatorItem.Add("");
            while (true)
            {
                string r = MonitorManager.GetOperatorType(i++);
                if (!IsNullOrWhiteSpace(r))
                    OperatorItem.Add(r);
                else break;
            }
        }

        public void Reset()
        {
            TimeStart = DateTime.Today;
            TimeEnd = DateTime.Now;
            OperationType = "";
            ObjectName = "";
            Message = "";
            OperatorItemIndex = 0;
            MessageTypeItemIndex = 0;
        }
    }
}