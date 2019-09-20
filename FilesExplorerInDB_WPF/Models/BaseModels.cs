using Command;
using FilesExplorerInDB_Manager.Interface;
using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static FilesExplorerInDB_WPF.AssemblyInformation;

namespace FilesExplorerInDB_WPF.Models
{
    public class BaseModels : INotifyPropertyChanged
    {
        protected static IFilesDbManager FilesDbManager { get; } = UnityContainerHelp.GetServer<IFilesDbManager>();
        protected static AssemblyInformation AppInformation { get; } = GetAssemblyInformation();

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly object _lock = new object();

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            lock (_lock)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}