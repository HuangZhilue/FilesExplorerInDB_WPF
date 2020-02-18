using System;
using System.Windows.Documents;

namespace FilesExplorerInDB_WPF.Models
{
    public class LogWindowModel : BaseModels
    {
        private DateTime _time;
        private string _messageType;
        private string _operationType;
        private string _operator;
        private string _objectName;
        private string _message;

        public DateTime Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        public string MessageType
        {
            get => _messageType;
            set
            {
                _messageType = value;
                OnPropertyChanged(nameof(MessageType));
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

        public string Operator
        {
            get => _operator;
            set
            {
                _operator = value;
                OnPropertyChanged(nameof(Operator));
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

        public static LogWindowModel GetInstance { get; } = new LogWindowModel();

        private LogWindowModel()
        {

        }
    }
}