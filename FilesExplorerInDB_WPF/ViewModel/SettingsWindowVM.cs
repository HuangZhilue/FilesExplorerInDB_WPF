using FilesExplorerInDB_WPF.Helper;
using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using static Resources.Properties.Settings.SettingType;
using static Resources.Resource;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace FilesExplorerInDB_WPF.ViewModel
{
    public class SettingsWindowVM : BaseViewModel
    {
        public SettingsWindowModel SettingsWindowModel { get; } = SettingsWindowModel.GetInstance;
        public ICommand CommandClose { get; }
        public ICommand CommandEnter { get; }
        public ICommand CommandCheckLocal { get; }
        public ICommand CommandDBTypeChange { get; }

        public static SettingsWindowVM GetInstance { get; } = new SettingsWindowVM();

        private SettingsWindowVM()
        {
            CommandClose = new DelegateCommand(Close);
            CommandEnter = new DelegateCommand(Enter);
            CommandCheckLocal = new DelegateCommand(CheckLocal);
            CommandDBTypeChange = new DelegateCommand(DBTypeChange);
            SetSettings_DBSetting();
        }

        private void SetSettings_DBSetting()
        {
            SettingsWindowModel.IsLocal = (bool) GetSetting(IsLocal);
            SettingsWindowModel.IsLocalText = SettingsWindowModel.IsLocal
                ? Settings_IsLocal_True
                : Settings_IsLocal_False;
            SettingsWindowModel.DBType = GetSetting(DBType).ToString();
            SettingsWindowModel.DBTypeText = $"{SettingsWindowModel.DBType}数据库";
            switch (SettingsWindowModel.DBType)
            {
                case "MySQL":
                    SettingsWindowModel.ConnectionString = GetSetting(ConnectionString4MySQL).ToString();
                    break;
                case "SQL Server":
                    SettingsWindowModel.ConnectionString = GetSetting(ConnectionString4MSSQL).ToString();
                    break;
                case "Oracle":
                    SettingsWindowModel.ConnectionString = GetSetting(ConnectionString4Oracle).ToString();
                    break;
                case "MongoDB":
                    SettingsWindowModel.ConnectionString = GetSetting(ConnectionString4MongoDB).ToString();
                    break;
                default:
                    throw new Exception(Message_ArgumentOutOfRangeException_DBType);
                //SettingsWindowModel.ConnectionString = "";
                //break;
            }

            if (SettingsWindowModel.IsLocal)
            {
                SettingsWindowModel.IsVisibilityFileStorageLocation = Visibility.Visible;
                SettingsWindowModel.FileStorageLocation = GetSetting(FileStorageLocation).ToString();
            }
            else
            {
                SettingsWindowModel.IsVisibilityFileStorageLocation = Visibility.Collapsed;
                //SettingsWindowModel.FileStorageLocation = "";
            }
        }

        private static void Close()
        {
            WindowManager.Remove(nameof(SettingsWindow));
        }

        private void CheckLocal()
        {
            SettingsWindowModel.IsVisibilityFileStorageLocation =
                SettingsWindowModel.IsLocal ? Visibility.Visible : Visibility.Collapsed;
            SettingsWindowModel.IsLocalText = SettingsWindowModel.IsLocal
                ? Settings_IsLocal_True
                : Settings_IsLocal_False;
        }

        private void DBTypeChange()
        {
            SettingsWindowModel.DBTypeText = $"{SettingsWindowModel.DBType}数据库";
            switch (SettingsWindowModel.DBType)
            {
                case "MySQL":
                    SettingsWindowModel.ConnectionString = GetSetting(ConnectionString4MySQL).ToString();
                    break;
                case "SQL Server":
                    SettingsWindowModel.ConnectionString = GetSetting(ConnectionString4MSSQL).ToString();
                    break;
                case "Oracle":
                    SettingsWindowModel.ConnectionString = GetSetting(ConnectionString4Oracle).ToString();
                    break;
                case "MongoDB":
                    SettingsWindowModel.ConnectionString = GetSetting(ConnectionString4MongoDB).ToString();
                    break;
                default:
                    throw new Exception(Message_ArgumentOutOfRangeException_DBType);
            }
        }

        private void Enter()
        {
            SaveSettings_DBSetting();

            Close();
        }

        private void SaveSettings_DBSetting()
        {
            SaveSetting(IsLocal, SettingsWindowModel.IsLocal);
            SaveSetting(DBType, SettingsWindowModel.DBType);
            switch (SettingsWindowModel.DBType)
            {
                case "MySQL":
                    SaveSetting(ConnectionString4MySQL, SettingsWindowModel.ConnectionString);
                    break;
                case "SQL Server":
                    SaveSetting(ConnectionString4MSSQL, SettingsWindowModel.ConnectionString);
                    break;
                case "Oracle":
                    SaveSetting(ConnectionString4Oracle, SettingsWindowModel.ConnectionString);
                    break;
                case "MongoDB":
                    SaveSetting(ConnectionString4MongoDB, SettingsWindowModel.ConnectionString);
                    break;
                default:
                    throw new Exception(Message_ArgumentOutOfRangeException_DBType);
            }

            SaveSetting(FileStorageLocation, SettingsWindowModel.FileStorageLocation);

            var result = MessageBox.Show("立即重启？", Caption_Info, MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}