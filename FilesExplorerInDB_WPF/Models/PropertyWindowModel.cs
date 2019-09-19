using System;
using System.Windows;
using System.Windows.Media;

namespace FilesExplorerInDB_WPF.Models
{
    public class PropertyWindowModel : BaseModels
    {
        private string _name;
        private string _type;
        private string _location;
        private string _size;
        private DateTime _creationTime;
        private DateTime _modifyTime;
        private DateTime _accessTime;
        private string _include;
        private Visibility _isVisibilityAccessTime;
        private Visibility _isVisibilityInclude;
        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
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

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public string Size
        {
            get => _size;
            set
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        public DateTime CreationTime
        {
            get => _creationTime;
            set
            {
                _creationTime = value;
                OnPropertyChanged(nameof(CreationTime));
            }
        }

        public DateTime ModifyTime
        {
            get => _modifyTime;
            set
            {
                _modifyTime = value;
                OnPropertyChanged(nameof(ModifyTime));
            }
        }

        public DateTime AccessTime
        {
            get => _accessTime;
            set
            {
                _accessTime = value;
                OnPropertyChanged(nameof(AccessTime));
            }
        }

        public string Include
        {
            get => _include;
            set
            {
                _include = value;
                OnPropertyChanged(nameof(Include));
            }
        }

        public Visibility IsVisibilityAccessTime
        {
            get => _isVisibilityAccessTime;
            set
            {
                _isVisibilityAccessTime = value;
                OnPropertyChanged(nameof(IsVisibilityAccessTime));
            }
        }

        public Visibility IsVisibilityInclude
        {
            get => _isVisibilityInclude;
            set
            {
                _isVisibilityInclude = value;
                OnPropertyChanged(nameof(IsVisibilityInclude));
            }
        }

        public static PropertyWindowModel GetInstance { get; } = new PropertyWindowModel();
        private PropertyWindowModel()
        {

        }
    }
}