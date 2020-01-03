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
        public ICommand CommandGetPath { get; }

        public static SettingsWindowVM GetInstance { get; } = new SettingsWindowVM();

        private SettingsWindowVM()
        {
            CommandClose = new DelegateCommand(Close);
            CommandEnter = new DelegateCommand(Enter);
            CommandCheckLocal = new DelegateCommand(CheckLocal);
            CommandDBTypeChange = new DelegateCommand(DBTypeChange);
            CommandGetPath = new DelegateCommand(GetPath);
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
            SettingsWindowModel.ConnectionString = GetSetting(SettingType.ConnectionString).ToString();
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
        }

        private void Enter()
        {
            SaveSettings_DBSetting();

            Close();
        }

        private void GetPath()
        {

        }

        private void SaveSettings_DBSetting()
        {
            SaveSetting(SettingType.IsLocal, SettingsWindowModel.IsLocal);
            SaveSetting(SettingType.DBType, SettingsWindowModel.DBType);
            SaveSetting(SettingType.ConnectionString, SettingsWindowModel.ConnectionString);
            SaveSetting(SettingType.FileStorageLocation, SettingsWindowModel.FileStorageLocation);
        }
    }
}