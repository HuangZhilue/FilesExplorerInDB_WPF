using System;
using System.IO;
using static System.String;

namespace Resources.Properties
{
    // 通过此类可以处理设置类的特定事件: 
    //  在更改某个设置的值之前将引发 SettingChanging 事件。
    //  在更改某个设置的值之后将引发 PropertyChanged 事件。
    //  在加载设置值之后将引发 SettingsLoaded 事件。
    //  在保存设置值之前将引发 SettingsSaving 事件。
    public sealed partial class Settings
    {
        private static Settings SettingsInstance { get; } = new Settings();

        public Settings()
        {
            // // 若要为保存和更改设置添加事件处理程序，请取消注释下列行: 
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // 在此处添加用于处理 SettingChangingEvent 事件的代码。
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 在此处添加用于处理 SettingsSaving 事件的代码。
        }

        /// <summary>
        /// 检查文件保存路径是否为空（为空则设置保存路径为该程序的根目录下）
        /// </summary>
        public static void CheckFileStorage()
        {
            if (SettingsInstance.FileStorageLocation is string fileStorageLocation &&
                IsNullOrEmpty(fileStorageLocation))
            {
                if (!Directory.Exists(fileStorageLocation))
                {
                    SaveSetting(SettingType.FileStorageLocation,
                        AppDomain.CurrentDomain.BaseDirectory + "FileStorageLocation\\");
                }
            }

            if (!Directory.Exists(SettingsInstance.FileStorageLocation))
            {
                SaveSetting(SettingType.FileStorageLocation,
                    AppDomain.CurrentDomain.BaseDirectory + "FileStorageLocation\\");
            }
        }

        public static void SaveSetting(SettingType type, object value)
        {
            switch (type)
            {
                case SettingType.FileStorageLocation:
                    SettingsInstance.FileStorageLocation = (string) value;
                    break;
                case SettingType.IsLocal:
                    SettingsInstance.IsLocal = (bool) value;
                    break;
                case SettingType.DBType:
                    SettingsInstance.DBType = (string) value;
                    break;
                case SettingType.ConnectionString4MySQL:
                    SettingsInstance.ConnectionString4MySQL = (string) value;
                    break;
                case SettingType.ConnectionString4MSSQL:
                    SettingsInstance.ConnectionString4MSSQL = (string) value;
                    break;
                case SettingType.ConnectionString4Oracle:
                    SettingsInstance.ConnectionString4Oracle = (string) value;
                    break;
                case SettingType.ConnectionString4MongoDB:
                    SettingsInstance.ConnectionString4MongoDB = (string) value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            SettingsInstance.Save();
        }

        public static object GetSetting(SettingType type)
        {
            switch (type)
            {
                case SettingType.FileStorageLocation:
                    if (IsNullOrWhiteSpace(SettingsInstance.FileStorageLocation))
                    {
                        CheckFileStorage();
                    }
                    return SettingsInstance.FileStorageLocation;
                case SettingType.IsLocal:
                    return SettingsInstance.IsLocal;
                case SettingType.DBType:
                    return SettingsInstance.DBType;
                case SettingType.ConnectionString4MySQL:
                    return SettingsInstance.ConnectionString4MySQL;
                case SettingType.ConnectionString4MSSQL:
                    return SettingsInstance.ConnectionString4MSSQL;
                case SettingType.ConnectionString4Oracle:
                    return SettingsInstance.ConnectionString4Oracle;
                case SettingType.ConnectionString4MongoDB:
                    return SettingsInstance.ConnectionString4MongoDB;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static string GetConnectionString()
        {
            var dbType = GetSetting(SettingType.DBType).ToString();
            switch (dbType)
            {
                case "MySQL":
                    return SettingsInstance.ConnectionString4MySQL;
                case "SQL Server":
                    return SettingsInstance.ConnectionString4MSSQL;
                case "Oracle":
                    return SettingsInstance.ConnectionString4Oracle;
                case "MongoDB":
                    return SettingsInstance.ConnectionString4MongoDB;
                default:
                    throw new Exception(Resource.Message_ArgumentOutOfRangeException_DBType);
            }
        }

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
    }
}