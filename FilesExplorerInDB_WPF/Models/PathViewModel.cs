using System;
using FilesExplorerInDB_EF.EFModels;
using System.Collections.Generic;
using System.Diagnostics;
using static Resources.Resource;

namespace FilesExplorerInDB_WPF.Models
{
    public class PathViewModel : BaseModels
    {
        private string _pathString;

        public string PathString
        {
            get => _pathString;
            set
            {
                _pathString = value;
                OnPropertyChanged(nameof(PathString));
            }
        }

        private string _searchString;

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged(nameof(SearchString));
            }
        }

        public System.Windows.Media.ImageSource PathBackImage => FilesDbManager.GetImage(chevron_left_regular_24);
        public System.Windows.Media.ImageSource PathNextImage => FilesDbManager.GetImage(chevron_right_regular_24);
        public System.Windows.Media.ImageSource PathPreviousImage => FilesDbManager.GetImage(chevron_up_regular_24);
        public System.Windows.Media.ImageSource CancelImage => FilesDbManager.GetImage(x_regular_24);
        public System.Windows.Media.ImageSource SearchImage => FilesDbManager.GetImage(search_alt_2_regular_24);

        /// <summary>
        /// 文件夹后退的堆栈
        /// </summary>
        private Stack<Folders> PreFolder { get; } = new Stack<Folders>();

        /// <summary>
        /// 文件夹前进的堆栈
        /// </summary>
        private Stack<Folders> FwdFolder { get; } = new Stack<Folders>();

        public static PathViewModel GetInstance { get; } = new PathViewModel();

        private PathViewModel()
        {

        }

        public void SetPathString(string folderId)
        {
            PathString = "";
            Stack<Folders> stack = FilesDbManager.GetRelativePath_Folder(folderId);
            foreach (Folders folder in stack)
            {
                PathString += folder.FolderName + "/";
            }
        }

        #region 历史路径跳转

        //TODO 跳转逻辑有点小问题

        /// <summary>
        /// 历史目录-后退
        /// </summary>
        public Folders Button_PathBack_Click(Folders folderNow)
        {
            if (PreFolder.Count <= 0) return null;
            if (FwdFolder.Count == 0 || folderNow != FwdFolder.Peek())
                FwdFolder.Push(folderNow);
            folderNow = PreFolder.Pop();
#if DEBUG
            string path = "PathBack--";
            foreach (Folders folder in PreFolder)
            {
                path += folder.FolderName + "/";
            }

            Debug.WriteLine(path);
            path = "";
            foreach (Folders folder in FwdFolder)
            {
                path += folder.FolderName + "/";
            }

            Debug.WriteLine(path);
#endif
            return folderNow;
        }

        /// <summary>
        /// 历史目录-前进
        /// </summary>
        public Folders Button_PathNext_Click(Folders folderNow)
        {
            if (FwdFolder.Count <= 0) return null;
            if (PreFolder.Count == 0 || folderNow != PreFolder.Peek())
                PreFolder.Push(folderNow);
            folderNow = FwdFolder.Pop();
#if DEBUG
            string path = "PathNext--";
            foreach (Folders folder in PreFolder)
            {
                path += folder.FolderName + "/";
            }

            Debug.WriteLine(path);
            path = "";
            foreach (Folders folder in FwdFolder)
            {
                path += folder.FolderName + "/";
            }

            Debug.WriteLine(path);
#endif
            return folderNow;
        }

        /// <summary>
        /// 历史目录-上一层
        /// </summary>
        public Folders Button_PathPrevious_Click(Folders folderNow)
        {
            if (folderNow == null) throw new Exception(Message_ArgumentNullException_Folders);
            Debug.WriteLine(folderNow.FolderId);
            if (folderNow.FolderId == App_RootLocalFolderId) return null;
            if (PreFolder.Count != 0 && folderNow != PreFolder.Peek())
                PreFolder.Push(folderNow);
            FwdFolder.Clear();
#if DEBUG
            string path = "PathPrev--";
            foreach (Folders folder in PreFolder)
            {
                path += folder.FolderName + "/";
            }

            Debug.WriteLine(path);
            path = "";
            foreach (Folders folder in FwdFolder)
            {
                path += folder.FolderName + "/";
            }

            Debug.WriteLine(path);
#endif
            return folderNow;
        }

        public void PathPush(Folders folderNow)
        {
            if (PreFolder.Count == 0 || folderNow != PreFolder.Peek())
                PreFolder.Push(folderNow);
#if DEBUG
            string path = "PathPush--";
            foreach (Folders folder in PreFolder)
            {
                path += folder.FolderName + "/";
            }

            Debug.WriteLine(path);
            path = "";
            foreach (Folders folder in FwdFolder)
            {
                path += folder.FolderName + "/";
            }

            Debug.WriteLine(path);
#endif
        }

        #endregion
    }
}