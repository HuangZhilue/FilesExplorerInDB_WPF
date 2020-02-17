using FilesExplorerInDB_Manager.Interface;
using JetBrains.Annotations;
using Resources.Properties;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static Command.UnityContainerHelp;
using static FilesExplorerInDB_WPF.AssemblyInformation;
using static Resources.Properties.Settings;

namespace FilesExplorerInDB_WPF.Models
{
    public class BaseModels : INotifyPropertyChanged
    {
        protected static IFilesDbManager FilesDbManager { get; } = GetServer<IFilesDbManager>();
        protected static IMonitorManager MonitorManager { get; } = GetServer<IMonitorManager>();
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

        protected static object GetSetting(SettingType type)
        {
            return Settings.GetSetting(type);
        }
    }
}