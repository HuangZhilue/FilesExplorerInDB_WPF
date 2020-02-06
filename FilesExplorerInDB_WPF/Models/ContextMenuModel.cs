using FilesExplorerInDB_Models.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using static Resources.Resource;

namespace FilesExplorerInDB_WPF.Models
{
    public class ContextMenuModel : BaseModels
    {
        private Visibility _isVisibleOpen = Visibility.Collapsed;
        private Visibility _isVisibleRefresh = Visibility.Collapsed;
        private Visibility _isVisibleCut = Visibility.Collapsed;
        private Visibility _isVisibleCopy = Visibility.Collapsed;
        private Visibility _isVisiblePaste = Visibility.Collapsed;
        private Visibility _isVisibleCreate = Visibility.Collapsed;
        private Visibility _isVisibleDelete = Visibility.Collapsed;
        private Visibility _isVisibleRename = Visibility.Collapsed;
        private Visibility _isVisibleProperty = Visibility.Collapsed;

        public ImageSource OpenImage => FilesDbManager.GetImage(folder_open_solid_24);
        public ImageSource RefreshImage => FilesDbManager.GetImage(revision_regular_24);
        public ImageSource CutImage => FilesDbManager.GetImage(cut_regular_24);
        public ImageSource CopyImage => FilesDbManager.GetImage(copy_alt_regular_24);
        public ImageSource PasteImage => FilesDbManager.GetImage(paste_regular_24);
        public ImageSource CreateImage => FilesDbManager.GetImage(plus_regular_24);
        public ImageSource DeleteImage => FilesDbManager.GetImage(trash_regular_24);
        public ImageSource RenameImage => FilesDbManager.GetImage(rename_regular_24);
        public ImageSource PropertyImage => FilesDbManager.GetImage(detail_regular_24);

        public Visibility IsVisibleOpen
        {
            get => _isVisibleOpen;
            set
            {
                _isVisibleOpen = value;
                OnPropertyChanged(nameof(IsVisibleOpen));
            }
        }

        public Visibility IsVisibleRefresh
        {
            get => _isVisibleRefresh;
            set
            {
                _isVisibleRefresh = value;
                OnPropertyChanged(nameof(IsVisibleRefresh));
            }
        }

        public Visibility IsVisibleCut
        {
            get => _isVisibleCut;
            set
            {
                _isVisibleCut = value;
                OnPropertyChanged(nameof(IsVisibleCut));
            }
        }

        public Visibility IsVisibleCopy
        {
            get => _isVisibleCopy;
            set
            {
                _isVisibleCopy = value;
                OnPropertyChanged(nameof(IsVisibleCopy));
            }
        }

        public Visibility IsVisiblePaste
        {
            get => _isVisiblePaste;
            set
            {
                _isVisiblePaste = value;
                OnPropertyChanged(nameof(IsVisiblePaste));
            }
        }

        public Visibility IsVisibleCreate
        {
            get => _isVisibleCreate;
            set
            {
                _isVisibleCreate = value;
                OnPropertyChanged(nameof(IsVisibleCreate));
            }
        }

        public Visibility IsVisibleDelete
        {
            get => _isVisibleDelete;
            set
            {
                _isVisibleDelete = value;
                OnPropertyChanged(nameof(IsVisibleDelete));
            }
        }

        public Visibility IsVisibleRename
        {
            get => _isVisibleRename;
            set
            {
                _isVisibleRename = value;
                OnPropertyChanged(nameof(IsVisibleRename));
            }
        }

        public Visibility IsVisibleProperty
        {
            get => _isVisibleProperty;
            set
            {
                _isVisibleProperty = value;
                OnPropertyChanged(nameof(IsVisibleProperty));
            }
        }

        public static ContextMenuModel GetInstance { get; } = new ContextMenuModel();

        private ContextMenuModel()
        {
        }

        public void SetMenuItems(List<ExplorerProperty> explorerList, bool isCopying, bool isCutting)
        {
            ReSetVisibility();
            if (explorerList != null && explorerList.Count > 0)
            {
                //打开
                //剪切
                //复制
                //删除
                //重命名
                //属性
                if (explorerList.Count == 1)
                {
                    IsVisibleOpen = Visibility.Visible;
                    IsVisibleRename = Visibility.Visible;
                    IsVisibleProperty = Visibility.Visible;
                    if (isCutting || isCopying)
                    {
                        IsVisiblePaste = Visibility.Visible;
                    }
                }

                IsVisibleDelete = Visibility.Visible;
                IsVisibleCut = Visibility.Visible;
                IsVisibleCopy = Visibility.Visible;
            }
            else
            {
                //刷新
                //新建
                //属性
                if (isCutting || isCopying)
                {
                    IsVisiblePaste = Visibility.Visible;
                }

                IsVisibleRefresh = Visibility.Visible;
                IsVisibleCreate = Visibility.Visible;
                IsVisibleProperty = Visibility.Visible;
            }
        }

        public void SetViewTabItems(List<ExplorerProperty> explorerList, bool isCopying, bool isCutting)
        {
            SetMenuItems(explorerList, isCopying, isCutting);
        }

        private void ReSetVisibility()
        {
            IsVisibleOpen = Visibility.Collapsed;
            IsVisibleRefresh = Visibility.Collapsed;
            IsVisibleCut = Visibility.Collapsed;
            IsVisibleCopy = Visibility.Collapsed;
            IsVisiblePaste = Visibility.Collapsed;
            IsVisibleCreate = Visibility.Collapsed;
            IsVisibleDelete = Visibility.Collapsed;
            IsVisibleRename = Visibility.Collapsed;
            IsVisibleProperty = Visibility.Collapsed;
        }
    }
}