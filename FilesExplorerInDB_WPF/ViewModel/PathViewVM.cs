using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_WPF.Models;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class PathViewVM : BaseViewModel
    {
        public PathViewModel PathViewModel { get; } = PathViewModel.GetInstance;

        public static PathViewVM GetInstance { get; } = new PathViewVM();

        private PathViewVM()
        {

        }

        /// <summary>
        /// 历史目录-后退
        /// </summary>
        public Folders PathBack(Folders folderNow)
        {
            return PathViewModel.Button_PathBack_Click(folderNow);
        }

        /// <summary>
        /// 历史目录-前进
        /// </summary>
        public Folders PathNext(Folders folderNow)
        {
            return PathViewModel.Button_PathNext_Click(folderNow);
        }

        /// <summary>
        /// 历史目录-上一层
        /// </summary>
        public Folders PathPrevious(Folders folderNow)
        {
            return PathViewModel.Button_PathPrevious_Click(folderNow);
        }

        public void PathPush(Folders folderNow)
        {
            PathViewModel.PathPush(folderNow);
        }

        public void SetPathString(string folderId)
        {
            PathViewModel.SetPathString(folderId);
        }
    }
}