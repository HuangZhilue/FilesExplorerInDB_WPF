using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Command;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Manager.Interface;

namespace FilesExplorerInDB_WPF
{
    /// <summary>
    /// PropertyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyWindow : Window
    {
        private readonly IFilesDbManager _filesDbManager;
        private readonly bool _isFolder;
        private readonly int _id;
        private string _defaultName;
        private Folders folders;
        private Files files;

        public PropertyWindow(object sender, bool isFolder, ImageSource imageSource, IFilesDbManager filesDbManager)
        {
            _filesDbManager = filesDbManager;
            _isFolder = isFolder;
            InitializeComponent();
            if (isFolder && sender is Folders folders)
            {
                _id = folders.FolderId;
                _defaultName = folders.FolderName;
                SetText_Folders(folders);
                this.folders = folders;
            }
            else if (!isFolder && sender is Files files)
            {
                _id = files.FileId;
                _defaultName = files.FileName;
                SetText_Files(files);
                this.files = files;
            }

            ImageBox.Source = imageSource;
        }

        private void SetText_Folders(Folders folders)
        {
            TextBox_Name.Text = folders.FolderName;
            TextBox_Type.Text = "文件夹";
            TextBox_Location.Text = GetLoction(folders.FolderLocalId);
            TextBox_Size.Text = _filesDbManager.DisplayFileSize(folders.Size) + "(" + folders.Size + " 字节)";
            TextBox_CreationTime.Text = folders.CreationTime.ToString("yyyy年M月d日，HH:mm:ss");
            TextBox_ModifyTime.Text = folders.ModifyTime.ToString("yyyy年M月d日，HH:mm:ss");
            TextBox_Include.Text = folders.FileIncludeCount + "个文件，" + folders.FolderIncludeCount + "个文件夹";
            TextBox_AccessTime.Visibility = Visibility.Hidden;
            Label_AccessTime.Visibility = Visibility.Hidden;
        }

        private void SetText_Files(Files files)
        {
            TextBox_Name.Text = files.FileName;
            TextBox_Type.Text = files.FileType;
            TextBox_Location.Text = GetLoction(files.FolderLocalId);
            TextBox_Size.Text = _filesDbManager.DisplayFileSize(files.Size) + "(" + files.Size + " 字节)";
            TextBox_CreationTime.Text = files.CreationTime.ToString("yyyy年M月d日，HH:mm:ss");
            TextBox_ModifyTime.Text = files.ModifyTime.ToString("yyyy年M月d日，HH:mm:ss");
            TextBox_AccessTime.Text = files.AccessTime.ToString("yyyy年M月d日，HH:mm:ss");
            TextBox_Include.Visibility = Visibility.Hidden;
            Label_Include.Visibility = Visibility.Hidden;
        }

        private string GetLoction(int folderId)
        {
            string path = "";
            Stack<Folders> stack = _filesDbManager.GetRelativePath_Folder(folderId);
            foreach (Folders folder in stack)
            {
                path += folder.FolderName + "/";
            }

            return path;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (_isFolder)
            {
                Folders folders = _filesDbManager.SetFoldersProperty(_id);
                SetText_Folders(folders);
            }
            else
            {
                Files files = _filesDbManager.SetFilesProperty(_id);
                if (!files.IsMiss)
                    SetText_Files(files);
                else
                {
                    MessageBoxResult result = MessageBox.Show("文件不存在或者文件读取失败！\n\r是否删除该文件？\n\rYes：删除\n\rNo：保留", "属性刷新错误",
                        MessageBoxButton.YesNo, MessageBoxImage.Error);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            bool deleteState = _filesDbManager.SetDeleteState(_id);
                            break;
                        case MessageBoxResult.No:
                            break;
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Rename();
            Close();
        }

        private void TextBox_Name_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rename();
            }
        }

        private void Rename()
        {
            if (_defaultName != TextBox_Name.Text)
            {
                MessageBoxResult result = MessageBox.Show("确定要修改文件、文件夹名称？", "提示", MessageBoxButton.OKCancel,
                    MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {
                    if (_isFolder)
                    {
                        _filesDbManager.Rename(folders, TextBox_Name.Text);
                    }
                    else
                    {
                        _filesDbManager.Rename(files, TextBox_Name.Text);
                    }

                    _defaultName = TextBox_Name.Text;
                }
                else
                {
                    TextBox_Name.Text = _defaultName;
                }
            }
        }
    }
}
