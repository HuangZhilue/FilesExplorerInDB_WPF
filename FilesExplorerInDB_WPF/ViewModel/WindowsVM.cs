using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using System.Windows.Input;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class WindowsVM : BaseViewModel
    {
        public ICommand CommandTabChange { get; }

        public WindowsModel WindowsModel { get; } = WindowsModel.GetInstance;
        public static WindowsVM GetInstance { get; } = new WindowsVM();

        private WindowsVM()
        {
            CommandTabChange = new DelegateCommand<object>(TabChange, m => true);
        }

        private void TabChange(object obj)
        {
            if (!(obj is int index)) return;
            switch (index)
            {
                case 0:
                    WindowsModel.FolderTreeWidth = 150f;
                    WindowsModel.ShowExplorer1 = true;
                    WindowsModel.ShowExplorer2 = false;
                    WindowsModel.ShowExplorer3 = false;
                    break;
                case 1:
                    WindowsModel.FolderTreeWidth = 0f;
                    WindowsModel.ShowExplorer1 = false;
                    WindowsModel.ShowExplorer2 = true;
                    WindowsModel.ShowExplorer3 = false;
                    break;
                case 2:
                    WindowsModel.FolderTreeWidth = 0f;
                    WindowsModel.ShowExplorer1 = false;
                    WindowsModel.ShowExplorer2 = false;
                    WindowsModel.ShowExplorer3 = true;
                    break;
                default:
                    WindowsModel.FolderTreeWidth = 150f;
                    WindowsModel.ShowExplorer1 = true;
                    WindowsModel.ShowExplorer2 = false;
                    WindowsModel.ShowExplorer3 = false;
                    break;
            }
        }
    }
}