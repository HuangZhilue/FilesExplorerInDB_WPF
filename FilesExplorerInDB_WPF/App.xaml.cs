using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Command;
using FilesExplorerInDB_Manager.Interface;
using FilesExplorerInDB_WPF.Helper;
using Resources;

namespace FilesExplorerInDB_WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static string ErrorType { get; set; } = "";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //注册Application_Error
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            if (e.Exception != null)
            {
#if DEBUG
                Debug.WriteLine($"Exception.Message:\t{e.Exception.Message}");
                Debug.WriteLine($"Exception.Source:\t{e.Exception.Source}");
                Debug.WriteLine($"Exception.StackTrace:\t{e.Exception.StackTrace}");

                MessageBox.Show(e.Exception.Message + "\r\n" + e.Exception.StackTrace, Resource.Caption_Error,
                    MessageBoxButton.OK, MessageBoxImage.Error);
#else
                MessageBox.Show(e.Exception.Message, Resource.Caption_Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
#endif
                GetInnerException(e.Exception);
                if (ErrorType == "无法连接")
                {
                    WindowManager.Register<SettingsWindow>(nameof(SettingsWindow));
                    WindowManager.Show(nameof(SettingsWindow), false);
                }
                else
                {
                    UnityContainerHelp.GetServer<IMonitorManager>().ErrorRecord(e.Exception);
                }
            }

            ErrorType = "";

            // Prevent default unhandled exception processing
            e.Handled = true;
        }

        void GetInnerException(Exception e)
        {
            if (e.InnerException != null)
            {
#if DEBUG
                Debug.WriteLine($"Exception.InnerException.Message:\t{e.InnerException.Message}");
                Debug.WriteLine($"Exception.InnerException.Source:\t{e.InnerException.Source}");
                Debug.WriteLine($"Exception.InnerException.StackTrace:\t{e.InnerException.StackTrace}");

                MessageBox.Show(e.InnerException.Message + "\r\n" + e.InnerException.StackTrace,
                    Resource.Caption_Error, MessageBoxButton.OK, MessageBoxImage.Error);
#else
                    MessageBox.Show(e.Exception.InnerException.Message, Resource.Caption_Error, MessageBoxButton.OK,
                        MessageBoxImage.Error);
#endif
                if (e.InnerException.Message.Contains("无法连接")) ErrorType = "无法连接";
                if (e.InnerException.InnerException != null) GetInnerException(e.InnerException);
            }
        }
    }
}