using System;
using System.Diagnostics;
using FilesExplorerInDB_WPF.Helper;
using System.Windows;
using FilesExplorerInDB_EF.EFModels;
using static Resources.Resource;
using static Resources.Properties.Settings;

namespace FilesExplorerInDB_WPF
{
    //查找DataTemplate生成的元素
    //https://docs.microsoft.com/en-us/dotnet/framework/wpf/data/how-to-find-datatemplate-generated-elements

    //WPF ListView 自动调整列宽
    //https://blog.csdn.net/djc11282/article/details/42261677

    //读取AssemblyInfo文件中的属性值
    //https://blog.csdn.net/ly_5683/article/details/88720182

    //Extract extra large Icon from a file, including network paths!
    //https://lluisfranco.com/2014/04/16/extract-extra-large-icon-from-a-file-including-network-paths/

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public MainWindow()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GetSetting(SettingType.RootFolderId).ToString()))
                {
                    SaveSetting(SettingType.RootFolderId, Guid.NewGuid().ToString());
                }

                InitializeComponent();

                WindowManager.Register<PropertyWindow>(nameof(PropertyWindow));
                WindowManager.Register<SettingsWindow>(nameof(SettingsWindow));
            }
            catch
            {
                using (var db = new FilesDB())
                {
                    var now = DateTime.Now;
                    db.Folders.Add(new Folders
                    {
                        FolderId = GetSetting(SettingType.RootFolderId).ToString(),
                        FolderName = "root",
                        ModifyTime = now,
                        CreationTime = now,
                        FolderLocalId = App_RootLocalFolderId,
                        Size = 0,
                        FileIncludeCount = 0,
                        FolderIncludeCount = 0,
                        IsDelete = false
                    });
                    db.Files.Add(new Files
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FileName = "null",
                        CreationTime = now,
                        ModifyTime = now,
                        AccessTime = now,
                        FolderLocalId = GetSetting(SettingType.RootFolderId).ToString(),
                        FileType = "null",
                        IsDelete = true,
                        IsMiss = true,
                        Size = 0,
                        RealName = "null",
                    });
                    db.Monitor.Add(new Monitor
                    {
                        MonitorId = Guid.NewGuid().ToString(), 
                        Operator = "System", 
                        MessageType = "Build",
                        OperationType = "Build", 
                        Message = "Initialize the directory structure",
                        ObjectName = "System",
                        Time = now
                    });
                    db.SaveChanges();
                }

                Process.Start(Application.ResourceAssembly.Location);
                Environment.Exit(0);
            }
        }
    }
}