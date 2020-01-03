using FilesExplorerInDB_WPF.Helper;
using System.Windows;
//using FilesExplorerInDB_EF.EFModels;

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
            //using (FilesDB db = new FilesDB())
            //{
            //    db.Folders.Add(new Folders { FolderId = 0, FolderName = "root" });
            //    db.Files.Add(new Files { FileName = "test" });
            //    db.Monitor.Add(new Monitor { Operator = "user", MessageType = "test", OperationType = "test" });
            //    db.SaveChanges();
            //}
            InitializeComponent();
            WindowManager.Register<PropertyWindow>(nameof(PropertyWindow));
            WindowManager.Register<SettingsWindow>(nameof(SettingsWindow));
        }
    }
}