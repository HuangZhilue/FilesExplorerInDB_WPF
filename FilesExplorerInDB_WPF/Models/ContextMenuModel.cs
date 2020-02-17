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
        private Visibility _isVisibleAllDelete = Visibility.Collapsed;
        private Visibility _isVisibleCompleteDelete = Visibility.Collapsed;
        private Visibility _isVisibleRename = Visibility.Collapsed;
        private Visibility _isVisibleProperty = Visibility.Collapsed;
        private Visibility _isVisibleTrashProperty = Visibility.Collapsed;
        private Visibility _isVisibleRestore = Visibility.Collapsed;

        public ImageSource OpenImage => FilesDbManager.GetImage(open);
        public ImageSource RefreshImage => FilesDbManager.GetImage(revision_regular_24);
        public ImageSource CutImage => FilesDbManager.GetImage(cut);
        public ImageSource CopyImage => FilesDbManager.GetImage(copy);
        public ImageSource PasteImage => FilesDbManager.GetImage(paste);
        public ImageSource CreateImage => FilesDbManager.GetImage(create);
        public ImageSource DeleteImage => FilesDbManager.GetImage(delete);
        public ImageSource RenameImage => FilesDbManager.GetImage(rename);
        public ImageSource PropertyImage => FilesDbManager.GetImage(property);

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

        public Visibility IsVisibleAllDelete
        {
            get => _isVisibleAllDelete;
            set
            {
                _isVisibleAllDelete = value;
                OnPropertyChanged(nameof(IsVisibleAllDelete));
            }
        }

        public Visibility IsVisibleCompleteDelete
        {
            get => _isVisibleCompleteDelete;
            set
            {
                _isVisibleCompleteDelete = value;
                OnPropertyChanged(nameof(IsVisibleCompleteDelete));
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

        public Visibility IsVisibleTrashProperty
        {
            get => _isVisibleTrashProperty;
            set
            {
                _isVisibleTrashProperty = value;
                OnPropertyChanged(nameof(IsVisibleTrashProperty));
            }
        }

        public Visibility IsVisibleRestore
        {
            get => _isVisibleRestore;
            set
            {
                _isVisibleRestore = value;
                OnPropertyChanged(nameof(IsVisibleRestore));
            }
        }

        public static ContextMenuModel GetInstance { get; } = new ContextMenuModel();
        public static ContextMenuModel GetTrashInstance { get; } = new ContextMenuModel();
        public static ContextMenuModel GetLogInstance { get; } = new ContextMenuModel();

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

        public void SetTrashMenuItems(List<ExplorerProperty> explorerList)
        {
            ReSetTrashVisibility();
            //清空
            IsVisibleAllDelete = Visibility.Visible;
            if (explorerList != null && explorerList.Count > 0)
            {
                //还原
                //删除
                //属性
                IsVisibleRestore = Visibility.Visible;
                IsVisibleCompleteDelete = Visibility.Visible;
                if (explorerList.Count == 1)
                {
                    IsVisibleTrashProperty = Visibility.Visible;
                }
            }
            else
            {
                //刷新
                IsVisibleRefresh = Visibility.Visible;
                //IsVisibleTrashProperty = Visibility.Visible;
            }
        }

        public void SetTrashTabItem(List<ExplorerProperty> trashList)
        {
            SetTrashMenuItems(trashList);
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

        private void ReSetTrashVisibility()
        {
            IsVisibleRestore = Visibility.Collapsed;
            IsVisibleRefresh = Visibility.Collapsed;
            IsVisibleCompleteDelete = Visibility.Collapsed;
            IsVisibleTrashProperty = Visibility.Collapsed;
            IsVisibleAllDelete = Visibility.Visible;
        }

        public void SetLogMenuItems(List<LogProperty> selectItem)
        {
            IsVisibleRefresh = Visibility.Visible;
            if (selectItem != null && selectItem.Count > 0)
            {
                IsVisibleOpen = Visibility.Visible;
            }
            else
            {
                IsVisibleOpen = Visibility.Collapsed;
            }
        }
    }
}