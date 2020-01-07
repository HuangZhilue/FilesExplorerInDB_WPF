using FilesExplorerInDB_WPF.Helper;
using FilesExplorerInDB_WPF.Models;
using Prism.Commands;
using Resources;
using System.Windows;
using System.Windows.Input;

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
            SettingsWindowModel.IsLocal = (bool) GetSetting(SettingType.IsLocal);
            SettingsWindowModel.IsLocalText = SettingsWindowModel.IsLocal
                ? Resource.Settings_IsLocal_True
                : Resource.Settings_IsLocal_False;
            SettingsWindowModel.DBType = GetSetting(SettingType.DBType).ToString();
            SettingsWindowModel.DBTypeText = $"{SettingsWindowModel.DBType}数据库";
            switch (SettingsWindowModel.DBType)
            {
                case "MySQL":
                    SettingsWindowModel.ConnectionString = GetSetting(SettingType.ConnectionString4MySQL).ToString();
                    break;
                case "SQL Server":
                    SettingsWindowModel.ConnectionString = GetSetting(SettingType.ConnectionString4MSSQL).ToString();
                    break;
                case "Oracle":
                    SettingsWindowModel.ConnectionString = GetSetting(SettingType.ConnectionString4Oracle).ToString();
                    break;
                case "MongoDB":
                    SettingsWindowModel.ConnectionString = GetSetting(SettingType.ConnectionString4MongoDB).ToString();
                    break;
                default:
                    SettingsWindowModel.ConnectionString = "";
                    break;
            }

            if (SettingsWindowModel.IsLocal)
            {
                SettingsWindowModel.IsVisibilityFileStorageLocation = Visibility.Visible;
                SettingsWindowModel.FileStorageLocation = GetSetting(SettingType.FileStorageLocation).ToString();
            }
            else
            {
                SettingsWindowModel.IsVisibilityFileStorageLocation = Visibility.Collapsed;
                SettingsWindowModel.FileStorageLocation = "";
            }
        }

        private static void Close()
        {
            WindowManager.Remove(nameof(SettingsWindow));
        }

        private void CheckLocal()
        {
            SettingsWindowModel.IsVisibilityFileStorageLocation = SettingsWindowModel.IsLocal ? Visibility.Collapsed : Visibility.Visible;
            SettingsWindowModel.IsLocalText = SettingsWindowModel.IsLocal
                ? Resource.Settings_IsLocal_True
                : Resource.Settings_IsLocal_False;
        }

        private void DBTypeChange()
        {
            SettingsWindowModel.DBTypeText = $"{SettingsWindowModel.DBType}数据库";
            switch (SettingsWindowModel.DBType)
            {
                case "MySQL":
                    SettingsWindowModel.ConnectionString = GetSetting(SettingType.ConnectionString4MySQL).ToString();
                    break;
                case "SQL Server":
                    SettingsWindowModel.ConnectionString = GetSetting(SettingType.ConnectionString4MSSQL).ToString();
                    break;
                case "Oracle":
                    SettingsWindowModel.ConnectionString = GetSetting(SettingType.ConnectionString4Oracle).ToString();
                    break;
                case "MongoDB":
                    SettingsWindowModel.ConnectionString = GetSetting(SettingType.ConnectionString4MongoDB).ToString();
                    break;
                default:
                    SettingsWindowModel.ConnectionString = "";
                    break;
            }
        }

        private void Enter()
        {
            SaveSettings_DBSetting();

            Close();
        }

        private void SaveSettings_DBSetting()
        {
            SaveSetting(SettingType.IsLocal, SettingsWindowModel.IsLocal);
            SaveSetting(SettingType.DBType, SettingsWindowModel.DBType);
            switch (SettingsWindowModel.DBType)
            {
                case "MySQL":
                    SaveSetting(SettingType.ConnectionString4MySQL, SettingsWindowModel.ConnectionString);
                    break;
                case "SQL Server":
                    SaveSetting(SettingType.ConnectionString4MSSQL, SettingsWindowModel.ConnectionString);
                    break;
                case "Oracle":
                    SaveSetting(SettingType.ConnectionString4Oracle, SettingsWindowModel.ConnectionString);
                    break;
                case "MongoDB":
                    SaveSetting(SettingType.ConnectionString4MongoDB, SettingsWindowModel.ConnectionString);
                    break;
            }
            SaveSetting(SettingType.FileStorageLocation, SettingsWindowModel.FileStorageLocation);

            var result = MessageBox.Show("立即重启？", Resource.Caption_Info, MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}