using Command;
using FilesExplorerInDB_Manager.Interface;
using FilesExplorerInDB_WPF.Properties;
using System;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public enum SettingType
    {
        FileStorageLocation
    }

    public class BaseViewModel
    {
        public readonly IFilesDbManager FilesDbManager = UnityContainerHelp.GetServer<IFilesDbManager>();
        private readonly Settings _settings = new Settings();

        protected void SaveSetting(SettingType type, object value)
        {
            switch (type)
            {
                case SettingType.FileStorageLocation:
                    _settings.FileStorageLocation = (string) value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            _settings.Save();
        }

        protected object GetSetting(SettingType type)
        {
            switch (type)
            {
                case SettingType.FileStorageLocation:
                    return _settings.FileStorageLocation;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}