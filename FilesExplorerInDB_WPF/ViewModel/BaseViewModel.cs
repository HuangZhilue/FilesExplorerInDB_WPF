using Command;
using FilesExplorerInDB_Manager.Interface;
using FilesExplorerInDB_WPF.Properties;
using System;
using System.IO;
using static System.String;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public enum SettingType
    {
        FileStorageLocation,
        IsLocal,
        DBType,
        ConnectionString
    }

    public class BaseViewModel
    {
        protected static IFilesDbManager FilesDbManager { get; } = UnityContainerHelp.GetServer<IFilesDbManager>();
        private static Settings Settings { get; } = new Settings();

        protected BaseViewModel()
        {
            CheckFileStorage();
        }

        protected static void SaveSetting(SettingType type, object value)
        {
            switch (type)
            {
                case SettingType.FileStorageLocation:
                    Settings.FileStorageLocation = (string) value;
                    break;
                case SettingType.IsLocal:
                    Settings.IsLocal = (bool)value;
                    break;
                case SettingType.DBType:
                    Settings.DBType = (string)value;
                    break;
                case SettingType.ConnectionString:
                    Settings.ConnectionString = (string)value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            Settings.Save();
        }

        protected static object GetSetting(SettingType type)
        {
            switch (type)
            {
                case SettingType.FileStorageLocation:
                    return Settings.FileStorageLocation;
                case SettingType.IsLocal:
                    return Settings.IsLocal;
                case SettingType.DBType:
                    return Settings.DBType;
                case SettingType.ConnectionString:
                    return Settings.ConnectionString;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// 检查文件保存路径是否为空（为空则设置保存路径为该程序的根目录下）
        /// </summary>
        private static void CheckFileStorage()
        {
            if (GetSetting(SettingType.FileStorageLocation) is string fileStorageLocation &&
                IsNullOrEmpty(fileStorageLocation))
            {
                if (!Directory.Exists(fileStorageLocation))
                {
                    SaveSetting(SettingType.FileStorageLocation,
                        AppDomain.CurrentDomain.BaseDirectory + "FileStorageLocation\\");
                }
            }

            if (!Directory.Exists(GetSetting(SettingType.FileStorageLocation) as string))
            {
                SaveSetting(SettingType.FileStorageLocation,
                    AppDomain.CurrentDomain.BaseDirectory + "FileStorageLocation\\");
            }
        }
    }
}