using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Models.Models;
using Resources;
using System;
using System.Windows.Media;
using static FilesExplorerInDB_WPF.AssemblyInformation;
using static Resources.Properties.Settings.SettingType;
using static Resources.Resource;

namespace FilesExplorerInDB_WPF.Models
{
    public class PropertyItems : BaseModels
    {
        private string _txtLabelName;
        private string _txtLabelType;
        private string _txtLabelOther1;
        private string _txtLabelOther2;
        private string _txtLabelOther3;
        private string _txtLabelOther4;
        private string _txtLabelOther5;
        private ImageSource _imageSource;

        public string TxtLabelName
        {
            get => _txtLabelName;
            set
            {
                _txtLabelName = value;
                OnPropertyChanged(nameof(TxtLabelName));
            }
        }

        public string TxtLabelType
        {
            get => _txtLabelType;
            set
            {
                _txtLabelType = value;
                OnPropertyChanged(nameof(TxtLabelType));
            }
        }

        public string TxtLabelOther1
        {
            get => _txtLabelOther1;
            set
            {
                _txtLabelOther1 = value;
                OnPropertyChanged(nameof(TxtLabelOther1));
            }
        }

        public string TxtLabelOther2
        {
            get => _txtLabelOther2;
            set
            {
                _txtLabelOther2 = value;
                OnPropertyChanged(nameof(TxtLabelOther2));
            }
        }

        public string TxtLabelOther3
        {
            get => _txtLabelOther3;
            set
            {
                _txtLabelOther3 = value;
                OnPropertyChanged(nameof(TxtLabelOther3));
            }
        }

        public string TxtLabelOther4
        {
            get => _txtLabelOther4;
            set
            {
                _txtLabelOther4 = value;
                OnPropertyChanged(nameof(TxtLabelOther4));
            }
        }

        public string TxtLabelOther5
        {
            get => _txtLabelOther5;
            set
            {
                _txtLabelOther5 = value;
                OnPropertyChanged(nameof(TxtLabelOther5));
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

        public static PropertyItems GetInstance = new PropertyItems();
        public static PropertyItems GetTrashInstance = new PropertyItems();

        private PropertyItems()
        {
        }

        #region 设置属性窗口

        /// <summary>
        /// 点击资源管理器项目获取属性
        /// </summary>
        public void GetPropertyFromExplorerItems(ExplorerProperty property)
        {
            if (property == null) throw new Exception(Message_ArgumentNullException_ExplorerProperty);
            TxtLabelName = property.Name;
            TxtLabelType = property.Type;
            TxtLabelOther2 = "创建时间：" + property.CreationTime;
            TxtLabelOther3 = "修改时间：" + property.ModifyTime;
            ImageSource = property.ImageSource;
            if (property.IsFolder)
            {
                TxtLabelOther1 = "文件夹大小：" + FilesDbManager.DisplayFileSize(property.Size) + "(" +
                                 property.Size + " 字节)";
                TxtLabelOther4 = "包含的文件夹数量：" + FilesDbManager.FoldersFind(property.Id).FolderIncludeCount;
                TxtLabelOther5 = "包含的文件数量：" + FilesDbManager.FoldersFind(property.Id).FileIncludeCount;
            }
            else
            {
                TxtLabelOther1 = "大小：" + FilesDbManager.DisplayFileSize(property.Size) + "(" + property.Size +
                                 " 字节)";
                TxtLabelOther4 = "访问时间：" + property.AccessTime;
                TxtLabelOther5 = "";
            }
        }

        /// <summary>
        /// 点击资源管理器(ListView)空白区域
        /// </summary>
        public void GetPropertyFromExplorerBlock(string nowFolderId)
        {
            if (nowFolderId != GetSetting(RootFolderId).ToString())
            {
                Folders folder = FilesDbManager.FoldersFind(nowFolderId);
                TxtLabelName = folder.FolderName;
                TxtLabelType = "文件夹";
                TxtLabelOther1 = "文件夹大小：" + FilesDbManager.DisplayFileSize(folder.Size) + "(" + folder.Size + " 字节)";
                TxtLabelOther2 = "创建时间：" + folder.CreationTime;
                TxtLabelOther3 = "修改时间：" + folder.ModifyTime;
                TxtLabelOther4 = "包含的文件夹数量：" + folder.FolderIncludeCount;
                TxtLabelOther5 = "包含的文件数量：" + folder.FileIncludeCount;
                ImageSource = FilesDbManager.GetImage(Resource.folder);
            }
            else
            {
                TxtLabelName = AssemblyTitle;
                TxtLabelType = "版本号：" + AssemblyVersion;
                TxtLabelOther1 = "说明" + AssemblyDescription;
                TxtLabelOther2 = "";
                TxtLabelOther3 = "";
                TxtLabelOther4 = "";
                TxtLabelOther5 = "";
                ImageSource = FilesDbManager.GetImage(explorer);
            }
        }

        #endregion
    }
}