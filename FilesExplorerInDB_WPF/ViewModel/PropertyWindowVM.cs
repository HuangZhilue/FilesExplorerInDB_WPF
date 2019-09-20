using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Models.Models;
using FilesExplorerInDB_WPF.Helper;
using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class PropertyWindowVM : BaseViewModel
    {
        public PropertyWindowModel PropertyWindowModel { get; } = PropertyWindowModel.GetInstance;
        public ICommand CommandClose { get; }
        public ICommand CommandRefresh { get; }
        public ICommand CommandEnter { get; }
        public ICommand CommandKeyDown { get; }

        private string NameBackup { get; set; } = "";
        private bool IsFolder { get; set; }
        private Folders Folders { get; set; }
        private Files Files { get; set; }

        public static PropertyWindowVM GetInstance { get; } = new PropertyWindowVM();

        private PropertyWindowVM()
        {
            CommandClose = new DelegateCommand(Close);
            CommandEnter = new DelegateCommand(Enter);
            CommandRefresh = new DelegateCommand(Refresh);
            CommandKeyDown = new DelegateCommand(Rename);
        }

        public void SetProperty(ExplorerProperty explorerProperty)
        {
            if (explorerProperty.IsFolder)
            {
                Folders folders = FilesDbManager.FoldersFind(explorerProperty.Id);
                SetText_Folders(folders);
                IsFolder = true;
            }
            else
            {
                Files files = FilesDbManager.FilesFind(explorerProperty.Id);
                SetText_Files(files);
                IsFolder = false;
            }

            PropertyWindowModel.ImageSource = explorerProperty.ImageSource;
        }

        private void SetText_Folders(Folders folders)
        {
            Folders = folders;
            PropertyWindowModel.Name = folders.FolderName;
            NameBackup = folders.FolderName;
            PropertyWindowModel.Type = "文件夹";
            PropertyWindowModel.Location = GetLocation(folders.FolderLocalId);
            PropertyWindowModel.Size = FilesDbManager.DisplayFileSize(folders.Size) + "(" + folders.Size + " 字节)";
            PropertyWindowModel.CreationTime = folders.CreationTime; //.ToString("yyyy年M月d日，HH:mm:ss");
            PropertyWindowModel.ModifyTime = folders.ModifyTime;
            PropertyWindowModel.Include = folders.FileIncludeCount + "个文件，" + folders.FolderIncludeCount + "个文件夹";
            PropertyWindowModel.IsVisibilityAccessTime = Visibility.Hidden;
            PropertyWindowModel.IsVisibilityInclude = Visibility.Visible;
        }

        private void SetText_Files(Files files)
        {
            Files = files;
            PropertyWindowModel.Name = files.FileName;
            NameBackup = files.FileName;
            PropertyWindowModel.Type = files.FileType;
            PropertyWindowModel.Location = GetLocation(files.FolderLocalId);
            PropertyWindowModel.Size = FilesDbManager.DisplayFileSize(files.Size) + "(" + files.Size + " 字节)";
            PropertyWindowModel.CreationTime = files.CreationTime;
            PropertyWindowModel.ModifyTime = files.ModifyTime;
            PropertyWindowModel.AccessTime = files.AccessTime;
            PropertyWindowModel.IsVisibilityInclude = Visibility.Hidden;
            PropertyWindowModel.IsVisibilityAccessTime = Visibility.Visible;
        }

        private static string GetLocation(int folderId)
        {
            string path = "";
            Stack<Folders> stack = FilesDbManager.GetRelativePath_Folder(folderId);
            foreach (Folders folder in stack)
            {
                path += folder.FolderName + "/";
            }

            return path;
        }

        private void Close()
        {
            WindowManager.Remove(nameof(PropertyWindow));
        }

        private void Refresh()
        {
            if (IsFolder)
            {
                Folders folders = FilesDbManager.SetFoldersProperty(Folders.FolderId);
                SetText_Folders(folders);
            }
            else
            {
                Files files = FilesDbManager.SetFilesProperty(Files.FileId);
                if (!files.IsMiss)
                    SetText_Files(files);
                else
                {
                    MessageBoxResult result = MessageBox.Show("文件不存在或者文件读取失败！\n\r是否删除该文件？\n\rYes：删除\n\rNo：保留", "属性刷新错误",
                        MessageBoxButton.YesNo, MessageBoxImage.Error);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            var deleteState = FilesDbManager.SetDeleteState(Files.FileId);
                            if (!deleteState)
                                MessageBox.Show("删除失败！");
                            break;
                        case MessageBoxResult.No:
                            break;
                        case MessageBoxResult.None:
                            break;
                        case MessageBoxResult.OK:
                            break;
                        case MessageBoxResult.Cancel:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void Enter()
        {
            Rename();
            Close();
        }

        private void Rename()
        {
            if (NameBackup == PropertyWindowModel.Name) return;
            MessageBoxResult result = MessageBox.Show("确定要修改文件、文件夹名称？", "提示", MessageBoxButton.OKCancel,
                MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                if (IsFolder)
                {
                    FilesDbManager.Rename(Folders, PropertyWindowModel.Name);
                }
                else
                {
                    FilesDbManager.Rename(Files, PropertyWindowModel.Name);
                }

                NameBackup = PropertyWindowModel.Name;
                WindowManager.SetDialogResult(nameof(PropertyWindow), true);
            }
            else
            {
                PropertyWindowModel.Name = NameBackup;
            }
        }
    }
}