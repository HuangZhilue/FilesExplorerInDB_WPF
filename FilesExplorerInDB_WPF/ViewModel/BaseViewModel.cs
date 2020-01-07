using Command;
using FilesExplorerInDB_Manager.Interface;
using Resources.Properties;
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
        ConnectionString4MySQL,
        ConnectionString4MSSQL,
        ConnectionString4Oracle,
        ConnectionString4MongoDB
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
                case SettingType.ConnectionString4MySQL:
                    Settings.ConnectionString4MySQL = (string)value;
                    break;
                case SettingType.ConnectionString4MSSQL:
                    Settings.ConnectionString4MSSQL = (string)value;
                    break;
                case SettingType.ConnectionString4Oracle:
                    Settings.ConnectionString4Oracle = (string)value;
                    break;
                case SettingType.ConnectionString4MongoDB:
                    Settings.ConnectionString4MongoDB = (string)value;
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
                case SettingType.ConnectionString4MySQL:
                    return Settings.ConnectionString4MySQL;
                case SettingType.ConnectionString4MSSQL:
                    return Settings.ConnectionString4MSSQL;
                case SettingType.ConnectionString4Oracle:
                    return Settings.ConnectionString4Oracle;
                case SettingType.ConnectionString4MongoDB:
                    return Settings.ConnectionString4MongoDB;
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