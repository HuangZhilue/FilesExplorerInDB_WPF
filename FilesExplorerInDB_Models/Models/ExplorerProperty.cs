using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FilesExplorerInDB_Models.Models
{
    public class ExplorerProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly object _lock = new object();
        private string _name;
        private bool _focusable;
        private bool _isFocused;
        private bool _isReadOnly = true;
        private string _id;
        private string _folderLocalId;
        private bool _isFolder;
        private string _type;
        private long _size;
        private string _creationTime;
        private string _modifyTime;
        private string _accessTime;
        private ImageSource _imageSource;
        private Thickness _borderThickness = new Thickness(0);
        private Cursor _cursor = Cursors.Arrow;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            lock (_lock)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string FolderLocalId
        {
            get => _folderLocalId;
            set
            {
                _folderLocalId = value;
                OnPropertyChanged(nameof(FolderLocalId));
            }
        }

        public bool IsFolder
        {
            get => _isFolder;
            set
            {
                _isFolder = value;
                OnPropertyChanged(nameof(IsFolder));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public long Size
        {
            get => _size;
            set
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        public string CreationTime
        {
            get => _creationTime;
            set
            {
                _creationTime = value;
                OnPropertyChanged(nameof(CreationTime));
            }
        }

        public string ModifyTime
        {
            get => _modifyTime;
            set
            {
                _modifyTime = value;
                OnPropertyChanged(nameof(ModifyTime));
            }
        }

        public string AccessTime
        {
            get => _accessTime;
            set
            {
                _accessTime = value;
                OnPropertyChanged(nameof(AccessTime));
            }
        }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public Thickness BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                OnPropertyChanged(nameof(BorderThickness));
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        public Cursor Cursor
        {
            get => _cursor;
            set
            {
                _cursor = value;
                OnPropertyChanged(nameof(Cursor));
            }
        }

        public bool Focusable
        {
            get => _focusable;
            set
            {
                _focusable = value;
                OnPropertyChanged(nameof(Focusable));
            }
        }

        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                _isFocused = value;
                OnPropertyChanged(nameof(IsFocused));
            }
        }
    }
}