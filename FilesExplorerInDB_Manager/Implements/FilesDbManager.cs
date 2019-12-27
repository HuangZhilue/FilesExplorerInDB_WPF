using Command;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using FilesExplorerInDB_Manager.Interface;
using FilesExplorerInDB_Models.Models;
using Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FilesExplorerInDB_Manager.Implements
{
    public class FilesDbManager : IFilesDbManager
    {
        private IFilesDbService DBService { get; } = UnityContainerHelp.GetServer<IFilesDbService>();
        private IFileIcon FileIcon { get; } = UnityContainerHelp.GetServer<IFileIcon>();
        private IMonitorManager MonitorManager { get; } = UnityContainerHelp.GetServer<IMonitorManager>();
        private ExplorerProperty Property { get; set; }
        private FilesDbManager DbManager { get; set; }

        #region 基础操作

        public Files FilesAdd(FileInfo fileInfo, int folderLocalId, string pathForSave)
        {
            if (fileInfo == null) throw new Exception(Resource.Message_ArgumentNullException_FileInfo);
            if (!Directory.Exists(pathForSave))
            {
                Directory.CreateDirectory(pathForSave);
            }

            Files files = (Files) Activator.CreateInstance(typeof(Files));
            files.FolderLocalId = folderLocalId;
            files.AccessTime = fileInfo.LastAccessTime;
            files.CreationTime = fileInfo.CreationTime;
            files.FileName = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf(".", StringComparison.Ordinal));
            files.FileType = fileInfo.Extension.Substring(1, fileInfo.Extension.Length - 1);
            files.ModifyTime = fileInfo.LastWriteTime;
            files.Size = fileInfo.Length;
            files = FilesAdd(files, true);
            string originName = fileInfo.FullName;
            fileInfo = fileInfo.CopyTo(pathForSave + "\\" + files.FileId + "." + files.FileType, true);
            files.RealName = fileInfo.FullName;
            FilesModified(files, true);
            MonitorManager.AddFileRecord(originName, files);
            return files;
        }

        private Files FilesAdd(Files entity, bool isSave)
        {
            entity = DBService.FilesAdd(entity);
            if (isSave)
                SaveChanges();
            return entity;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return DBService.FilesFind(keyValue);
        }

        public List<Files> LoadFilesEntites(Expression<Func<Files, bool>> where)
        {
            return DBService.LoadFilesEntites(where).ToList();
        }

        private Folders FoldersAdd(Folders entity, bool isSave)
        {
            entity = DBService.FoldersAdd(entity);
            if (isSave)
                SaveChanges();
            return entity;
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return DBService.FoldersFind(keyValue);
        }

        public List<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> where)
        {
            return DBService.LoadFoldersEntites(where).ToList();
        }

        private void FilesModified(Files files, bool isSave = false)
        {
            DBService.FilesModified(files);
            if (isSave) SaveChanges();
        }

        private void FoldersModified(Folders folder, bool isSave = false)
        {
            DBService.FoldersModified(folder);
            if (isSave) SaveChanges();
        }

        private int SaveChanges()
        {
            return DBService.SaveChanges();
        }

        #endregion

        public List<ExplorerProperty> SetExplorerItemsList(int localFolderId, out Folders folderNow)
        {
            if (localFolderId < 0) localFolderId = 0;
            var list = new List<ExplorerProperty>();
            folderNow = FoldersFind(localFolderId);
            Debug.WriteLine(folderNow);
            folderNow.FolderNodes = LoadFoldersEntites(f => f.FolderLocalId == localFolderId && f.IsDelete == false)
                .ToList();
            foreach (var folder in folderNow.FolderNodes)
            {
                list.Add(SetExplorerItems_Folders(folder, GetImage(Resource.folder)));
            }

            var imageBitmap = Resource.DEFAULT;
            foreach (var file in folderNow.Files)
            {
                if (file.IsDelete) continue;
                if (file.IsMiss) imageBitmap = Resource.fileNotFount;
                list.Add(SetExplorerItems_Files(file, imageBitmap, Resource.fileNotFount));
                imageBitmap = Resource.DEFAULT;
            }

            return list;
        }

        public ExplorerProperty SetExplorerItem(Folders folderNow)
        {
            if (folderNow == null) throw new Exception(Resource.Message_ArgumentNullException_Folders);
            Debug.WriteLine(folderNow);
            var e = SetExplorerItems_Folders(folderNow, GetImage(Resource.folder));
            return e;
        }

        /// <summary>
        /// 设置文件信息
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="defaultBitmap">默认的文件类型图标</param>
        /// <param name="errorBitmap">错误标识的图标</param>
        /// <returns>文件信息</returns>
        private ExplorerProperty SetExplorerItems_Files(Files file, Bitmap defaultBitmap, Bitmap errorBitmap)
        {
            Property = (ExplorerProperty) Activator.CreateInstance(typeof(ExplorerProperty));
            Property.Id = file.FileId;
            Property.FolderLocalId = file.FolderLocalId;
            Property.IsFolder = false;
            Property.Name = file.FileName;
            Property.AccessTime = file.AccessTime.ToString(CultureInfo.CurrentCulture);
            Property.CreationTime = file.CreationTime.ToString(CultureInfo.CurrentCulture);
            Property.ModifyTime = file.ModifyTime.ToString(CultureInfo.CurrentCulture);
            Property.Size = file.Size;
            Property.Type = file.FileType;
            if (file.RealName != null && File.Exists(file.RealName))
            {
                Property.ImageSource =
                    GetImage(FileIcon.GetBitmapFromFilePath(file.RealName,
                        Implements.FileIcon.IconSizeEnum.ExtraLargeIcon));
            }
            else if (file.RealName == null)
            {
                Property.ImageSource = GetImage(defaultBitmap);
            }
            else
            {
                SetFilesProperty(file.FileId);
                Property.ImageSource = GetImage(errorBitmap);
            }

            return Property;
        }

        /// <summary>
        /// 设置文件夹信息
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="imageSource">文件夹图标</param>
        /// <returns>文件夹信息</returns>
        private ExplorerProperty SetExplorerItems_Folders(Folders folder, ImageSource imageSource)
        {
            Property = (ExplorerProperty)Activator.CreateInstance(typeof(ExplorerProperty));
            Property.Id = folder.FolderId;
            Property.FolderLocalId = folder.FolderLocalId;
            Property.IsFolder = true;
            Property.Name = folder.FolderName;
            Property.AccessTime = "";
            Property.CreationTime = folder.CreationTime.ToString("yyyy/MM/dd HH:mm", CultureInfo.CurrentCulture);
            Property.ModifyTime = folder.ModifyTime.ToString("yyyy/MM/dd HH:mm", CultureInfo.CurrentCulture);
            Property.Size = folder.Size;
            Property.Type = Resource.Property_Type_Folder;
            Property.ImageSource = imageSource;
            return Property;
        }

        /// <summary>
        /// 通过递归的方式设置目录及其子目录
        /// </summary>
        /// <param name="folderId">父目录ID</param>
        /// <param name="folders">除了父目录外的其他目录数据</param>
        /// <returns>父目录数据</returns>
        public List<Folders> GetFoldersTree(int folderId, List<Folders> folders)
        {
            List<Folders> mainFolders = folders.Where(f => f.FolderLocalId == folderId && f.IsDelete == false).ToList();
            List<Folders> otherFolders =
                folders.Where(f => f.FolderLocalId != folderId && f.IsDelete == false).ToList();
            foreach (Folders chillerFolder in mainFolders)
            {
                chillerFolder.FolderNodes = GetFoldersTree(chillerFolder.FolderId, otherFolders);
            }

            return mainFolders;
        }

        #region 粘贴

        /// <summary>
        /// 文件、文件夹粘贴
        /// </summary>
        /// <param name="folderForPaste">粘贴到所在文件夹的ID</param>
        /// <param name="items">要粘贴的项目</param>
        /// <param name="isCutting">是否为剪切</param>
        /// <returns>是否成功</returns>
        public bool Paste(int folderForPaste, List<ExplorerProperty> items, bool isCutting)
        {
            if (items == null) throw new Exception(Resource.Message_ArgumentNullException_ExplorerPropertyList);
            var lid = 0;
            foreach (var item in items)
            {
                if (isCutting) //剪切
                {
                    if (item.IsFolder)
                    {
                        #region 防止剪切出现剪切粘贴到被剪切的项目里面

                        var idForCheck = folderForPaste;
                        while (true)
                        {
                            if (idForCheck == -1) break;
                            var f = FoldersFind(idForCheck);
                            if (f.FolderId == item.Id) return false;
                            idForCheck = f.FolderLocalId;
                        }

                        #endregion

                        Folders folder = FoldersFind(item.Id);
                        var oldId = folder.FolderLocalId;
                        folder.FolderLocalId = folderForPaste;
                        FoldersModified(folder);
                        MonitorManager.CutFolderRecord(folder, oldId.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        Files file = FilesFind(item.Id);
                        var oldId = file.FolderLocalId;
                        file.FolderLocalId = folderForPaste;
                        FilesModified(file);
                        MonitorManager.CutFileRecord(file, oldId.ToString(CultureInfo.CurrentCulture));
                    }
                }
                else //复制
                {
                    if (item.IsFolder)
                    {
                        Folders folder = FoldersFind(item.Id);
                        PasteChild(folderForPaste, folder.FolderId);
                        MonitorManager.CopyFolderRecord(folder, folderForPaste.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        Files file = FilesFind(item.Id);
                        var oldLocalId = file.FolderLocalId;
                        file.FolderLocalId = folderForPaste;
                        //Files file2 = UnityContainerHelp.GetServer<Files>();
                        //file2.FolderLocalId = folderForPaste;
                        //file.AccessTime = DateTime.Now;
                        file.CreationTime = DateTime.Now;
                        //file2.FileName = file.FileName;
                        //file2.FileType = file.FileType;
                        //file2.IsDelete = file.IsDelete;
                        //file2.ModifyTime = DateTime.Now;
                        //file2.Size = file.Size;
                        //file2.RealName = file.RealName;
                        FilesAdd(file, false);
                        MonitorManager.CopyFileRecord(file, oldLocalId.ToString(CultureInfo.CurrentCulture));
                    }
                }

                lid = item.FolderLocalId;
            }

            var s = SaveChanges();
            SetParentFoldersProperty(lid);
            return s > 0;
        }

        /// <summary>
        /// 粘贴（子文件夹与子文件）
        /// </summary>
        /// <param name="folderForPaste">要粘贴到的文件夹的ID</param>
        /// <param name="folderId">原文件夹的ID</param>
        private void PasteChild(int folderForPaste, int folderId)
        {
            DbManager = new FilesDbManager();

            Folders folder = FoldersFind(folderId); //原文件夹
            Folders folder2 = (Folders) Activator.CreateInstance(folder.GetType());// UnityContainerHelp.GetServer<Folders>();
            folder2.FolderLocalId = folderForPaste;
            folder2.CreationTime = DateTime.Now;
            folder2.FileIncludeCount = folder.FileIncludeCount;
            folder2.FolderIncludeCount = folder.FolderIncludeCount;
            folder2.FolderName = folder.FolderName;
            folder2.IsDelete = folder.IsDelete;
            folder2.ModifyTime = DateTime.Now;
            folder2.Size = folder.Size;
            folder2 = DbManager.FoldersAdd(folder2, true);
            List<Folders> foldersList = LoadFoldersEntites(f => f.FolderLocalId == folder.FolderId);
            foreach (var folders in foldersList) //获取原文件夹的子文件夹
            {
                PasteChild(folder2.FolderId, folders.FolderId);
            }

            List<Files> filesList = LoadFilesEntites(f => f.FolderLocalId == folder.FolderId);
            foreach (var files in filesList) //获取原文件夹下的文件
            {
                //Files file = UnityContainerHelp.GetServer<Files>();
                files.FolderLocalId = folder2.FolderId;
                //file.AccessTime = DateTime.Now;
                files.CreationTime = DateTime.Now;
                //file.FileName = files.FileName;
                //file.FileType = files.FileType;
                //file.IsDelete = files.IsDelete;
                //file.ModifyTime = DateTime.Now;
                //file.Size = files.Size;
                //file.RealName = files.RealName;
                DbManager.FilesAdd(files, false);
            }

            DbManager.SaveChanges();
        }

        #endregion

        #region 删除

        /// <summary>
        /// 将项目标记为删除状态
        /// </summary>
        /// <param name="items">要删除的项目</param>
        /// <returns>成功状态</returns>
        public bool SetDeleteState(List<ExplorerProperty> items)
        {
            if (items == null) throw new Exception(Resource.Message_ArgumentNullException_ExplorerPropertyList);
            int lid = 0;
            foreach (ExplorerProperty item in items)
            {
                if (item.IsFolder)
                {
                    Folders folder = FoldersFind(item.Id);
                    folder.IsDelete = true;
                    FoldersModified(folder);
                    MonitorManager.DeleteFolderRecord(folder);
                }
                else
                {
                    Files file = FilesFind(item.Id);
                    file.IsDelete = true;
                    FilesModified(file);
                    MonitorManager.DeleteFileRecord(file);
                }

                lid = item.FolderLocalId;
            }

            int s = SaveChanges();
            SetParentFoldersProperty(lid);
            return s > 0;
        }

        /// <summary>
        /// 将项目标记为删除状态
        /// </summary>
        /// <param name="filesForDelete">要删除的文件的ID</param>
        /// <returns>成功状态</returns>
        public bool SetDeleteState(int filesForDelete)
        {
            Files file = FilesFind(filesForDelete);
            file.IsDelete = true;
            FilesModified(file, true);
            SetParentFoldersProperty(file.FolderLocalId);
            MonitorManager.DeleteFileRecord(file);
            return true;
        }

        #endregion

        #region 重命名

        /// <summary>
        /// 将项目重命名
        /// </summary>
        /// <param name="items">要重命名的项目</param>
        /// <param name="newName">新的名称</param>
        /// <returns>成功状态</returns>
        public bool Rename(List<ExplorerProperty> items, string newName)
        {
            if (items == null) throw new Exception(Resource.Message_ArgumentNullException_ExplorerPropertyList);
            foreach (ExplorerProperty item in items)
            {
                if (item.IsFolder)
                {
                    Folders folder = FoldersFind(item.Id);
                    var oldName = folder.FolderName;
                    folder.FolderName = newName;
                    folder.ModifyTime = DateTime.Now;
                    FoldersModified(folder);
                    MonitorManager.RenameFolderRecord(folder, oldName);
                }
                else
                {
                    Files file = FilesFind(item.Id);
                    var oldName = file.FileName;
                    file.FileName = newName;
                    file.ModifyTime = DateTime.Now;
                    FilesModified(file);
                    MonitorManager.RenameFileRecord(file, oldName);
                }
            }

            return SaveChanges() > 0;
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="file">要重命名的文件</param>
        /// <param name="newName">新名称</param>
        public void Rename(Files file, string newName)
        {
            if (file == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var oldName = file.FileName;
            file.FileName = newName;
            file.ModifyTime = DateTime.Now;
            FilesModified(file, true);
            MonitorManager.RenameFileRecord(file, oldName);
        }

        /// <summary>
        /// 重命名文件夹
        /// </summary>
        /// <param name="folder">要重命名的文件夹</param>
        /// <param name="newName">新名称</param>
        public void Rename(Folders folder, string newName)
        {
            if (folder == null) throw new Exception(Resource.Message_ArgumentNullException_Folders);
            var oldName = folder.FolderName;
            folder.FolderName = newName;
            folder.ModifyTime = DateTime.Now;
            FoldersModified(folder, true);
            MonitorManager.RenameFolderRecord(folder, oldName);
        }

        #endregion

        #region 新建文件夹

        /// <summary>
        /// 在当前目录下新建文件夹
        /// </summary>
        /// <param name="parentFoldersId">父文件夹ID</param>
        /// <returns>建立好的新文件夹</returns>
        public Folders CreateFolders(int parentFoldersId)
        {
            Folders folder = UnityContainerHelp.GetServer<Folders>();
            folder.FolderLocalId = parentFoldersId;
            folder.CreationTime = DateTime.Now;
            folder.FileIncludeCount = 0;
            folder.FolderIncludeCount = 0;
            folder.FolderName = "新建文件夹" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff", CultureInfo.CurrentCulture);
            folder.IsDelete = false;
            folder.ModifyTime = DateTime.Now;
            folder.Size = 0;
            folder = FoldersAdd(folder, true);
            SetParentFoldersProperty(parentFoldersId);
            MonitorManager.AddFolderRecord(folder);
            return folder;
        }

        #endregion

        #region 设置属性

        #region 设置文件夹属性

        /// <summary>
        /// 刷新子文件夹的属性
        /// </summary>
        /// <param name="foldersId">父文件夹ID</param>
        /// <returns>父文件夹</returns>
        public Folders SetFoldersProperty(int foldersId)
        {
            Folders parentFolders = FoldersFind(foldersId);
            //将需要叠加统计的数据，初始化为0，否则将导致下一次叠加统计时数据错误。
            parentFolders.FileIncludeCount = 0;
            parentFolders.FolderIncludeCount = 0;
            parentFolders.Size = 0;
            parentFolders = SetChildFoldersProperty(parentFolders, -1);
            SaveChanges();
            return parentFolders;
        }

        /// <summary>
        /// 刷新子文件夹的属性
        /// </summary>
        /// <param name="parentFolders">父文件夹</param>
        /// <param name="endFolderId">最终的文件夹（当设置子文件夹到最终文件夹时，停止继续设置子文件夹，避免设置整个文件夹系统）</param>
        /// <returns>子文件夹的属性</returns>
        private Folders SetChildFoldersProperty(Folders parentFolders, int endFolderId)
        {
            List<Folders> folders =
                LoadFoldersEntites(f => f.FolderLocalId == parentFolders.FolderId && !f.IsDelete).ToList();
            List<Folders> foldersList = new List<Folders>();
            if (parentFolders.FolderId != endFolderId) //当刷新子文件夹到最终文件夹时，停止继续刷新子文件夹
                foreach (Folders childFolders in folders)
                {
                    //将需要叠加统计的数据，初始化为0，否则将导致下一次叠加统计时数据错误。
                    childFolders.FileIncludeCount = 0;
                    childFolders.FolderIncludeCount = 0;
                    childFolders.Size = 0;
                    foldersList.Add(SetChildFoldersProperty(childFolders, endFolderId));
                }

            List<Files> files = LoadFilesEntites(f => f.FolderLocalId == parentFolders.FolderId && !f.IsDelete)
                .ToList();
            files = SetFilesListProperty(files);
            if (!folders.Any())
            {
                parentFolders.FileIncludeCount = files.Count;
                parentFolders.FolderIncludeCount = folders.Count;
                parentFolders.Size = files.Select(f => f.Size).Sum();
            }
            else
            {
                parentFolders.FileIncludeCount += files.Count;
                parentFolders.FolderIncludeCount += folders.Count;
                parentFolders.Size += files.Select(f => f.Size).Sum();
            }

            parentFolders.FileIncludeCount += foldersList.Select(f => f.FileIncludeCount).Sum();
            parentFolders.FolderIncludeCount += foldersList.Select(f => f.FolderIncludeCount).Sum();
            parentFolders.Size += foldersList.Select(f => f.Size).Sum();
            parentFolders.ModifyTime = DateTime.Now;
            FoldersModified(parentFolders);
            return parentFolders;
        }

        /// <summary>
        /// 刷新父文件夹的属性
        /// </summary>
        /// <param name="foldersId">子项目所在文件夹</param>
        /// <returns>父文件夹</returns>
        private Folders SetParentFoldersProperty(int foldersId)
        {
            if (foldersId == -1)
            {
                SaveChanges();
                return null;
            }

            List<Folders> folders =
                LoadFoldersEntites(f => f.FolderLocalId == foldersId && !f.IsDelete).ToList();
            List<Files> files = LoadFilesEntites(f => f.FolderLocalId == foldersId && !f.IsDelete).ToList();
            files = SetFilesListProperty(files);
            Folders folder = FoldersFind(foldersId);
            folder.FileIncludeCount = folders.Sum(f => f.FileIncludeCount) + files.Count;
            folder.FolderIncludeCount = folders.Sum(f => f.FolderIncludeCount) + folders.Count;
            folder.Size = folders.Sum(f => f.Size) + files.Sum(f => f.Size);
            FoldersModified(folder);
            return SetParentFoldersProperty(folder.FolderLocalId);
        }

        #endregion

        #region 设置文件属性

        private List<Files> SetFilesListProperty(List<Files> filesList)
        {
            List<Files> filesList2 = new List<Files>();
            foreach (Files files in filesList)
            {
                filesList2.Add(SetFilesProperty(files.FileId));
            }

            return filesList2;
        }

        /// <summary>
        /// 刷新文件属性
        /// </summary>
        /// <param name="filesId">文件ID</param>
        /// <returns>文件属性</returns>
        public Files SetFilesProperty(int filesId)
        {
            Files files = FilesFind(filesId);
            if (files?.RealName != null)
            {
                FileInfo fileInfo = new FileInfo(files.RealName);
                if (fileInfo.Exists)
                {
                    files.AccessTime = fileInfo.LastAccessTime;
                    files.CreationTime = fileInfo.CreationTime;
                    files.ModifyTime = fileInfo.LastWriteTime;
                }
                else
                {
                    files.IsMiss = true;
                    files.ModifyTime = DateTime.Now;
                    MonitorManager.SetMissStateRecord(files);
                }
            }
            else if (files != null)
            {
                files.IsMiss = true;
                files.ModifyTime = DateTime.Now;
                files.Size = 0;
                MonitorManager.SetMissStateRecord(files);
            }

            FilesModified(files, true);
            return files;
        }

        #endregion

        #endregion

        #region 获取文件夹的相对路径

        public Stack<Folders> GetRelativePath_Folder(int folderId)
        {
            Stack<Folders> stack = new Stack<Folders>();
            while (folderId != -1)
            {
                Folders folders = FoldersFind(folderId);
                stack.Push(folders);
                folderId = folders.FolderLocalId;
            }

            return stack;
        }

        #endregion

        #region 转化文件图标

        /// <summary>
        /// 通过本地化的资源图像文件，返回适用于Image控件的图像
        /// </summary>
        /// <param name="imageBitmap">System.Drawing.Bitmap 类型的本地化资源</param>
        /// <returns>图像的System.Windows.Media.ImageSource</returns>
        public ImageSource GetImage(Bitmap imageBitmap)
        {
            if (imageBitmap == null) return null;
            return Imaging.CreateBitmapSourceFromHBitmap(
                imageBitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
        }

        #endregion

        #region 转换文件大小信息

        private const long KB = 1024;
        private const long MB = KB * 1024;
        private const long GB = MB * 1024;

        /// <summary>
        /// 转换文件大小信息
        /// </summary>
        /// <param name="size">Long（Int64）格式的文件字节大小</param>
        /// <returns>转换成功的文件大小信息</returns>
        public string DisplayFileSize(long size)
        {
            if (size >= GB)
            {
                return ((double) size / GB).ToString("0.00", CultureInfo.CurrentCulture) + " GB";
            }

            if (size >= MB)
            {
                double value = (double) size / MB;
                return value.ToString("0.00", CultureInfo.CurrentCulture) + " MB";
            }

            if (size >= KB)
            {
                double value = (double) size / KB;
                return value.ToString("0.00", CultureInfo.CurrentCulture) + " KB";
            }

            if (size == -1)
            {
                return "";
            }

            return size.ToString("0.00", CultureInfo.CurrentCulture) + " B";
        }

        #endregion
    }
}