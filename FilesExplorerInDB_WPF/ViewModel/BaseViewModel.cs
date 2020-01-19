using FilesExplorerInDB_Manager.Interface;
using Resources.Properties;
using static Command.UnityContainerHelp;
using static Resources.Properties.Settings;

namespace FilesExplorerInDB_WPF.ViewModel
{

    public class BaseViewModel
    {
        protected static IFilesDbManager FilesDbManager { get; } = GetServer<IFilesDbManager>();

        protected BaseViewModel()
        {
        }

        protected static void SaveSetting(SettingType type, object value)
        {
            Settings.SaveSetting(type, value);
        }

        protected static object GetSetting(SettingType type)
        {
            return Settings.GetSetting(type);
        }
    }
}