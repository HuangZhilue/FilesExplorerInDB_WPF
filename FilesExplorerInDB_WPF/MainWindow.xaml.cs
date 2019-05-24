using System;
using Command;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Manager.Interface;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FilesExplorerInDB_Models.Models;
using FilesExplorerInDB_WPF.Properties;
using static System.Double;
using Brush = System.Windows.Media.Brush;

namespace FilesExplorerInDB_WPF
{
    //查找DataTemplate生成的元素
    //https://docs.microsoft.com/en-us/dotnet/framework/wpf/data/how-to-find-datatemplate-generated-elements

    //WPF ListView 自动调整列宽
    //https://blog.csdn.net/djc11282/article/details/42261677



    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 相关字段

        private readonly IFilesDbManager _filesDbManager = UnityContainerHelp.GetServer<IFilesDbManager>();
        private readonly AssemblyInformation _appInformation = AssemblyInformation.GetAssemblyInformation();

        /// <summary>
        /// 当前整个文件夹数据库数据
        /// </summary>
        private List<Folders> _folders;

        /// <summary>
        /// ListView当前被选中的项目
        /// </summary>
        private readonly List<ExplorerProperty> _selectItems = new List<ExplorerProperty>();

        /// <summary>
        /// 文件夹后退的堆栈
        /// </summary>
        private readonly Stack<Folders> _preFolder = new Stack<Folders>();

        /// <summary>
        /// 文件夹前进的堆栈
        /// </summary>
        private readonly Stack<Folders> _fwdFolder = new Stack<Folders>();

        /// <summary>
        /// 当前正在展示的文件夹
        /// </summary>
        private Folders _folderNow;

        /// <summary>
        /// 标识当前状态是否正在复制
        /// </summary>
        private bool _isCutting;

        /// <summary>
        /// 标识当前状态是否正在剪切
        /// </summary>
        private bool _isCopying;

        /// <summary>
        /// 标识当前ListView是否被多选
        /// </summary>
        private bool _isMultipleSelection;

        /// <summary>
        /// 原文件名（文件夹名），用于恢复
        /// </summary>
        private string _nameBackup;

        /// <summary>
        /// 搜索控件时指示的当前索引值（为0）
        /// </summary>
        private int _defaultIndexOfObj;

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CheckFileStorage();
            SetExplorer_TreeView();
            SetExplorer_ListView(0);
            ListView_Explorer_MouseLeftButtonDown(null, null);
        }

        #endregion

        #region 设置目录树

        /// <summary>
        /// 设置目录树
        /// </summary>
        private void SetExplorer_TreeView()
        {
            _folders = _filesDbManager.LoadFoldersEntites(f => f.FolderId != -1).ToList();
            TreeView_Explorer.ItemsSource = _filesDbManager.GetFoldersTree(-1, _folders);
        }

        #endregion

        #region 设置资源管理器

        /// <summary>
        /// 根据父文件夹ID来设置资源管理器的内容
        /// </summary>
        /// <param name="folderId">父文件夹ID</param>
        private void SetExplorer_ListView(int folderId)
        {
            _folderNow = _filesDbManager.FoldersFind(folderId);
            ListView_Explorer.Items.Clear();
            List<Folders> explorerFolders =
                _filesDbManager.LoadFoldersEntites(f => f.FolderLocalId == folderId && !f.IsDelete).ToList();
            ListViewItem item;
            foreach (var folder in explorerFolders)
            {
                item = new ListViewItem
                {
                    Content = _filesDbManager.SetExplorerItems_Folders(folder,
                        _filesDbManager.GetImage(Properties.Resources.folder))
                };
                item.MouseDoubleClick += ListView_Explorer_Folder_MouseDoubleClick; //添加鼠标双击事件（打开文件夹）
                item.PreviewMouseLeftButtonDown +=
                    ListView_Explorer_Property_PreviewMouseLeftButtonDown; //添加鼠标左键单击事件（显示属性）
                item.PreviewMouseRightButtonDown +=
                    ListView_Explorer_Folder_PreviewMouseRightButtonDown; //添加鼠标右键单击事件（显示右键菜单-针对文件夹的右键菜单）
                ListView_Explorer.Items.Add(item);
            }

            List<Files> explorerFiles =
                _filesDbManager.LoadFilesEntites(f => f.FolderLocalId == folderId && !f.IsDelete).ToList();
            foreach (var file in explorerFiles)
            {
                Bitmap imageBitmap = Properties.Resources.DEFAULT;
                if (file.IsMiss) imageBitmap = Properties.Resources.fileNotFount;
                item = new ListViewItem
                {
                    Content = _filesDbManager.SetExplorerItems_Files(file, imageBitmap)
                };
                if (file.IsMiss)
                    item.Background = (Brush) new BrushConverter().ConvertFromString("#4CFF0000");
                item.PreviewMouseLeftButtonDown +=
                    ListView_Explorer_Property_PreviewMouseLeftButtonDown; //添加鼠标左键单击事件（显示属性）
                item.PreviewMouseRightButtonDown +=
                    ListView_Explorer_File_PreviewMouseRightButtonDown; //添加鼠标右键单击事件（显示右键菜单-针对文件的右键菜单）
                ListView_Explorer.Items.Add(item);
            }

            ColumnWidthAuto();

            string path = "";
            Stack<Folders> stack = _filesDbManager.GetRelativePath_Folder(folderId);
            foreach (Folders folder in stack)
            {
                path += folder.FolderName + "/";
            }

            TextBox_Path.Text = path;
        }

        /// <summary>
        /// 资源管理器双击打开下一级文件夹
        /// </summary>
        private void ListView_Explorer_Folder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((ListViewItem) sender).Content is ExplorerProperty property &&
                property.IsFolder)
            {
                _preFolder.Push(_folderNow);
                SetExplorer_ListView(property.Id);
            }
        }

        #endregion

        #region 历史路径跳转

        /// <summary>
        /// 历史目录-后退
        /// </summary>
        private void Button_PathBack_Click(object sender, RoutedEventArgs e)
        {
            if (_preFolder.Count <= 0) return;
            Folders back = _preFolder.Pop();
            _fwdFolder.Push(_folderNow);
            SetExplorer_ListView(back.FolderId);
        }

        /// <summary>
        /// 历史目录-前进
        /// </summary>
        private void Button_PathNext_Click(object sender, RoutedEventArgs e)
        {
            if (_fwdFolder.Count <= 0) return;
            Folders next = _fwdFolder.Pop();
            _preFolder.Push(_folderNow);
            SetExplorer_ListView(next.FolderId);
        }

        /// <summary>
        /// 历史目录-上一层
        /// </summary>
        private void Button_PathPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (_folderNow.FolderId == 0) return;
            _preFolder.Push(_folderNow);
            _fwdFolder.Clear();
            SetExplorer_ListView(_folderNow.FolderLocalId);
            ListView_Explorer_MouseLeftButtonDown(null, null);
        }

        #endregion

        #region 设置属性窗口

        /// <summary>
        /// 点击资源管理器项目获取属性
        /// </summary>
        private void ListView_Explorer_Property_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((ListViewItem) sender).Content is ExplorerProperty property)
            {
                Label_Name.Text = property.Name;
                Label_Type.Text = property.Type;
                Label_Other_2.Text = "创建时间：" + property.CreationTime;
                Label_Other_3.Text = "修改时间：" + property.ModifyTime;
                Image_PropertyType.Source = property.ImageSource;
                if (property.IsFolder)
                {
                    Label_Other_1.Text = "文件夹大小：" + _filesDbManager.DisplayFileSize(property.Size) + "(" +
                                         property.Size + " 字节)";
                    Label_Other_4.Text = "包含的文件夹数量：" + _filesDbManager.FoldersFind(property.Id).FolderIncludeCount;
                    Label_Other_5.Text = "包含的文件数量：" + _filesDbManager.FoldersFind(property.Id).FileIncludeCount;
                }
                else
                {
                    Label_Other_1.Text = "大小：" + _filesDbManager.DisplayFileSize(property.Size) + "(" + property.Size +
                                         " 字节)";
                    Label_Other_4.Text = "访问时间：" + property.AccessTime;
                    Label_Other_5.Text = "";
                }
            }
        }

        /// <summary>
        /// 点击资源管理器(ListView)空白区域
        /// </summary>
        private void ListView_Explorer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListView_Explorer.SelectedItems.Clear();
            if (_folderNow != null && _folderNow.FolderId != 0)
            {
                Label_Name.Text = _folderNow.FolderName;
                Label_Type.Text = "文件夹";
                Label_Other_1.Text = "文件夹大小：" +_filesDbManager.DisplayFileSize(_folderNow.Size) + "(" + _folderNow.Size + " 字节)";
                Label_Other_2.Text = "创建时间：" + _folderNow.CreationTime;
                Label_Other_3.Text = "修改时间：" + _folderNow.ModifyTime;
                Label_Other_4.Text = "包含的文件夹数量：" + _folderNow.FolderIncludeCount;
                Label_Other_5.Text = "包含的文件数量：" + _folderNow.FileIncludeCount;
                Image_PropertyType.Source = _filesDbManager.GetImage(Properties.Resources.folder);
            }
            else
            {
                Label_Name.Text = _appInformation.AssemblyTitle;
                Label_Type.Text = "版本号：" + _appInformation.AssemblyVersion;
                Label_Other_1.Text = "说明" + _appInformation.AssemblyDescription;
                Label_Other_2.Text = "";
                Label_Other_3.Text = "";
                Label_Other_4.Text = "";
                Label_Other_5.Text = "";
                Image_PropertyType.Source = _filesDbManager.GetImage(Properties.Resources.explorer);
            }

            ListView_Explorer.Focus();
        }

        #endregion

        #region 目录树跳转

        /// <summary>
        /// 双击目录树进行跳转
        /// </summary>
        private void TreeView_Explorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Folders folders = (Folders) (e.OriginalSource as TextBlock)?.DataContext;
            if (folders != null)
            {
                _preFolder.Push(_folderNow);
                _fwdFolder.Clear();
                SetExplorer_ListView(folders.FolderId);
                ListView_Explorer_MouseLeftButtonDown(null, null);
            }
        }

        #endregion

        #region 右键菜单管理

        #region 资源管理器右键菜单

        /// <summary>
        /// 文件夹的右键单击事件（打开右键菜单）
        /// </summary>
        private void ListView_Explorer_Folder_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //打开
            //剪切
            //复制
            //删除
            //重命名
            //属性
            if (ListView_Explorer.SelectedItems.Count > 1)
            {
                _isMultipleSelection = true;
            }

            ListViewItem item = (ListViewItem) sender;
            ContextMenu menu = new ContextMenu();

            if (!_isMultipleSelection)
            {
                MenuItem openMenuItem = new MenuItem
                {
                    Header = "打开",
                    DataContext = item.Content
                };
                openMenuItem.Click += OpenFolder;
                menu.Items.Add(openMenuItem);
            }

            MenuItem cutMenuItem = new MenuItem
            {
                Header = "剪切",
                DataContext = item.Content
            };
            cutMenuItem.Click += Cut;
            menu.Items.Add(cutMenuItem);

            MenuItem copyMenuItem = new MenuItem
            {
                Header = "复制",
                DataContext = item.Content
            };
            copyMenuItem.Click += Copy;
            menu.Items.Add(copyMenuItem);

            if ((_isCutting || _isCopying) && !_isMultipleSelection)
            {
                MenuItem pasteMenuItem = new MenuItem
                {
                    Header = "粘贴",
                    DataContext = item.Content
                };
                pasteMenuItem.Click += Paste;
                menu.Items.Add(pasteMenuItem);
            }

            MenuItem deleteMenuItem = new MenuItem
            {
                Header = "删除",
                DataContext = item.Content
            };
            deleteMenuItem.Click += Delete;
            menu.Items.Add(deleteMenuItem);

            if (!_isMultipleSelection)
            {
                MenuItem renameMenuItem = new MenuItem
                {
                    Header = "重命名",
                    DataContext = item.Content
                };
                renameMenuItem.Click += Rename;
                menu.Items.Add(renameMenuItem);

                MenuItem propertyMenuItem = new MenuItem
                {
                    Header = "属性",
                    DataContext = item.Content
                };
                propertyMenuItem.Click += FolderProperty;
                menu.Items.Add(propertyMenuItem);
            }

            item.ContextMenu = menu;
            _isMultipleSelection = false;
        }

        /// <summary>
        /// 文件的右键单击事件（打开右键菜单）
        /// </summary>
        private void ListView_Explorer_File_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //打开
            //剪切
            //复制
            //删除
            //重命名
            //属性
            if (ListView_Explorer.SelectedItems.Count > 1)
            {
                _isMultipleSelection = true;
            }

            ListViewItem item = (ListViewItem) sender;
            ContextMenu menu = new ContextMenu();

            if (!_isMultipleSelection)
            {
                MenuItem openMenuItem = new MenuItem
                {
                    Header = "打开",
                    DataContext = item.Content
                };
                openMenuItem.Click += OpenFile;
                menu.Items.Add(openMenuItem);
            }

            MenuItem cutMenuItem = new MenuItem
            {
                Header = "剪切",
                DataContext = item.Content
            };
            cutMenuItem.Click += Cut;
            menu.Items.Add(cutMenuItem);

            MenuItem copyMenuItem = new MenuItem
            {
                Header = "复制",
                DataContext = item.Content
            };
            copyMenuItem.Click += Copy;
            menu.Items.Add(copyMenuItem);

            MenuItem deleteMenuItem = new MenuItem
            {
                Header = "删除",
                DataContext = item.Content
            };
            deleteMenuItem.Click += Delete;
            menu.Items.Add(deleteMenuItem);

            if (!_isMultipleSelection)
            {
                MenuItem renameMenuItem = new MenuItem
                {
                    Header = "重命名",
                    DataContext = item.Content
                };
                renameMenuItem.Click += Rename;
                menu.Items.Add(renameMenuItem);

                MenuItem propertyMenuItem = new MenuItem
                {
                    Header = "属性",
                    DataContext = item.Content
                };
                propertyMenuItem.Click += FileProperty;
                menu.Items.Add(propertyMenuItem);
            }

            item.ContextMenu = menu;
            _isMultipleSelection = false;
        }

        /// <summary>
        /// 资源管理器空白区域右键单击事件（打开右键菜单）
        /// </summary>
        private void ListView_Explorer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListView_Explorer.SelectedItems.Clear();
            //刷新
            //新建
            //属性
            ListView item = (ListView) sender;
            ContextMenu menu = new ContextMenu();

            MenuItem refreshMenuItem = new MenuItem
            {
                Header = "刷新"
            };
            refreshMenuItem.Click += FolderRefresh;
            menu.Items.Add(refreshMenuItem);

            if (_isCutting || _isCopying)
            {
                MenuItem pasteMenuItem = new MenuItem
                {
                    Header = "粘贴"
                };
                pasteMenuItem.Click += Paste;
                menu.Items.Add(pasteMenuItem);
            }


            MenuItem createMenuItem = new MenuItem
            {
                Header = "新建文件夹"
            };
            createMenuItem.Click += FolderCreate;
            menu.Items.Add(createMenuItem);

            MenuItem propertyMenuItem = new MenuItem
            {
                Header = "属性"
            };
            propertyMenuItem.Click += FolderProperty;
            menu.Items.Add(propertyMenuItem);


            item.ContextMenu = menu;
        }

        #endregion

        #region 通用菜单

        /// <summary>
        /// 剪切
        /// </summary>
        private void Cut(object sender, RoutedEventArgs e)
        {
            _selectItems.Clear();
            foreach (ListViewItem item in ListView_Explorer.SelectedItems)
            {
                _selectItems.Add(item.Content as ExplorerProperty);
            }

            _isCopying = false;
            _isCutting = true;
        }

        /// <summary>
        /// 复制
        /// </summary>
        private void Copy(object sender, RoutedEventArgs e)
        {
            _selectItems.Clear();
            foreach (ListViewItem item in ListView_Explorer.SelectedItems)
            {
                _selectItems.Add(item.Content as ExplorerProperty);
            }

            _isCopying = true;
            _isCutting = false;
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        private void Paste(object sender, RoutedEventArgs e)
        {
            int folderIdForPaste;
            if ((ListView_Explorer.SelectedItem as ListViewItem)?.Content is ExplorerProperty property)
            {
                folderIdForPaste = property.Id;
            }
            else
            {
                folderIdForPaste = _folderNow.FolderId;
            }

            _filesDbManager.Paste(folderIdForPaste, _selectItems, _isCutting);
            if (_isCutting) _selectItems.Clear();
            SetExplorer_TreeView();
            SetExplorer_ListView(_folderNow.FolderId);
            _isCopying = false;
            _isCutting = false;
        }

        /// <summary>
        /// 删除
        /// </summary>
        private void Delete(object sender, RoutedEventArgs e)
        {
            _selectItems.Clear();
            foreach (ListViewItem item in ListView_Explorer.SelectedItems)
            {
                _selectItems.Add(item.Content as ExplorerProperty);
            }

            _filesDbManager.SetDeleteState(_selectItems);
            _isCopying = false;
            _isCutting = false;
            _selectItems.Clear();
            SetExplorer_TreeView();
            SetExplorer_ListView(_folderNow.FolderId);
        }

        /// <summary>
        /// 重命名
        /// </summary>
        private void Rename(object sender, RoutedEventArgs e)
        {
            //_isRenaming = true;
            if (ListView_Explorer.SelectedItem is ListViewItem item)
            {
                _selectItems.Clear();
                ExplorerProperty property = item.Content as ExplorerProperty;
                _selectItems.Add(property);
                _defaultIndexOfObj = 0;

                var myListBoxItem = (ListViewItem) ListView_Explorer.ItemContainerGenerator.ContainerFromItem(item);

                // Getting the ContentPresenter of myListBoxItem
                var myContentPresenter =
                    FindVisualChild<ContentPresenter>(myListBoxItem,
                        1); //因该方法所要求的TextBox控件在第二个DataTemplate中，故需要设置索引为1。（ 索引0为的Image控件(展示文件图标用) ）

                // Finding textBlock from the DataTemplate that is set on that ContentPresenter
                DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;

                var obj = myDataTemplate.FindName("nameTextBox", myContentPresenter); //nameTextBox 是在模板内定义的 x:Name

                if (obj is TextBox nameTextBox)
                {
                    //...do something
                    //BorderThickness="0" IsReadOnly="True" Background="{x:Null}" BorderBrush="{x:Null}" Cursor="Arrow" Focusable="False"
                    TextBox defaultTextBox = new TextBox();
                    nameTextBox.BorderThickness = new Thickness(1);
                    nameTextBox.IsReadOnly = defaultTextBox.IsReadOnly;
                    nameTextBox.Background = defaultTextBox.Background;
                    nameTextBox.BorderBrush = defaultTextBox.BorderBrush;
                    nameTextBox.Cursor = defaultTextBox.Cursor;
                    nameTextBox.Focusable = defaultTextBox.Focusable;
                    nameTextBox.Focus();
                    nameTextBox.SelectionStart = nameTextBox.Text.Length;
                    _nameBackup = nameTextBox.Text;
                }
            }
        }

        /// <summary>
        /// 寻找子标签（子控件）
        /// </summary>
        /// <typeparam name="TChildItem">子控件类型</typeparam>
        /// <param name="obj">子控件</param>
        /// <param name="indexOfObj">该控件的索引</param>
        /// <returns>子控件</returns>
        private TChildItem FindVisualChild<TChildItem>(DependencyObject obj, int indexOfObj)
            where TChildItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is TChildItem item)
                {
                    if (_defaultIndexOfObj == indexOfObj) return item;
                    else
                    {
                        _defaultIndexOfObj++;
                        TChildItem childOfChild = FindVisualChild<TChildItem>(child, indexOfObj);
                        if (childOfChild != null)
                            return childOfChild;
                    }
                }
                else
                {
                    TChildItem childOfChild = FindVisualChild<TChildItem>(child, indexOfObj);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }

        /// <summary>
        /// 完成重命名操作
        /// </summary>
        private void NameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox nameTextBox)
                {
                    _filesDbManager.Rename(_selectItems, nameTextBox.Text);

                    //_isRenaming = false;
                    _selectItems.Clear();
                    NameTextBox_LostFocus(sender, new RoutedEventArgs());
                    _nameBackup = "";
                    SetExplorer_TreeView();
                    SetExplorer_ListView(_folderNow.FolderId);
                }
            }
        }

        /// <summary>
        /// 失去焦点，取消重命名操作
        /// </summary>
        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox nameTextBox)
            {
                nameTextBox.BorderThickness = new Thickness(0);
                nameTextBox.IsReadOnly = true;
                nameTextBox.Background = null;
                nameTextBox.BorderBrush = null;
                nameTextBox.Cursor = Cursors.Arrow;
                nameTextBox.Focusable = false;
                nameTextBox.Text = _nameBackup;
            }

            //_isRenaming = false;
        }

        #endregion

        #region 文件夹右键菜单管理

        /// <summary>
        /// 打开文件夹
        /// </summary>
        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            if (((MenuItem) sender).DataContext is ExplorerProperty property && property.IsFolder)
            {
                _preFolder.Push(_folderNow);
                SetExplorer_ListView(property.Id);
            }
        }

        /// <summary>
        /// 文件夹属性
        /// </summary>
        private void FolderProperty(object sender, RoutedEventArgs e)
        {
            if (((MenuItem) sender).DataContext is ExplorerProperty property)
            {
                new PropertyWindow(_filesDbManager.FoldersFind(property.Id), property.IsFolder, property.ImageSource,
                    _filesDbManager).Show();
            }
        }

        #endregion

        #region 文件右键菜单管理

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            if (((MenuItem) sender).DataContext is ExplorerProperty property)
            {
                string path = Settings.Default.FileStorageLocation + property.Id + "." + property.Type;
                if (File.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    MessageBox.Show("文件物理路径错误", "打开文件出错", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// 文件属性
        /// </summary>
        private void FileProperty(object sender, RoutedEventArgs e)
        {
            if (((MenuItem) sender).DataContext is ExplorerProperty property)
            {
                new PropertyWindow(_filesDbManager.FilesFind(property.Id), property.IsFolder, property.ImageSource,
                    _filesDbManager).Show();
            }
        }

        #endregion

        #region 资源管理器右键菜单管理

        /// <summary>
        /// 刷新资源管理器
        /// </summary>
        private void FolderRefresh(object sender, RoutedEventArgs e)
        {
            SetExplorer_TreeView();
            SetExplorer_ListView(_folderNow.FolderId);
        }

        /// <summary>
        /// 新建文件夹
        /// </summary>
        private void FolderCreate(object sender, RoutedEventArgs e)
        {
            Folders folders = _filesDbManager.CreateFolders(_folderNow.FolderId);
            SetExplorer_TreeView();
            SetExplorer_ListView(_folderNow.FolderId);

            foreach (ListViewItem item in ListView_Explorer.Items)
            {
                if (item.Content is ExplorerProperty property)
                {
                    if (property.Id == folders.FolderId && property.IsFolder)
                    {
                        item.IsSelected = true;
                        ListView_Explorer.UpdateLayout(); //更新布局，否则在获取组件时报NULL
                        Rename(null, null);
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region 列宽自适应

        /// <summary>
        /// 列宽自适应
        /// </summary>
        private void ColumnWidthAuto()
        {
            if (!(ListView_Explorer.View is GridView gv)) return;
            foreach (var gvColumn in gv.Columns)
            {
                gvColumn.Width = 100;
                gvColumn.Width = NaN;
            }
        }

        #endregion

        private void Button_Setting_Click(object sender, RoutedEventArgs e)
        {

        }

        #region 文件拖放

        private void ListView_Explorer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        /// <summary>
        /// 文件拖放并保存
        /// </summary>
        private void ListView_Explorer_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                String[] files = (String[]) e.Data.GetData(DataFormats.FileDrop);

                if (files != null)
                    foreach (String s in files)
                    {
                        if (File.Exists(s))
                        {
                            // 是文件
                            FileInfo fi = new FileInfo(s);

                            CheckFileStorage();

                            _filesDbManager.FilesAdd(fi, _folderNow.FolderId, Settings.Default.FileStorageLocation);
                            SetExplorer_ListView(_folderNow.FolderId);
                        }
                        else if (Directory.Exists(s))
                        {
                            // 是文件夹
                            MessageBox.Show("暂不支持文件夹拖放操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            // 都不是
                            MessageBox.Show("未检测到文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
            }
        }

        #endregion

        /// <summary>
        /// 刷新整个文件系统的数据
        /// </summary>
        private void Button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            _filesDbManager.SetFoldersProperty(0);
            SetExplorer_TreeView();
            SetExplorer_ListView(0);
        }

        /// <summary>
        /// 检查文件保存路径是否为空（为空则设置保存路径为该程序的根目录下）
        /// </summary>
        private static void CheckFileStorage()
        {
            if (String.IsNullOrEmpty(Settings.Default.FileStorageLocation))
            {
                Settings.Default.FileStorageLocation =
                    AppDomain.CurrentDomain.BaseDirectory + "FileStorageLocation\\";
                Settings.Default.Save();
            }
        }
    }
}
