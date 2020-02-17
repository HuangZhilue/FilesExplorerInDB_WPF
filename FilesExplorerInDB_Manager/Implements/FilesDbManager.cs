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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Command.UnityContainerHelp;
using static FilesExplorerInDB_Manager.Implements.FileIcon.IconSizeEnum;
using static Resources.Properties.Settings;
using static Resources.Resource;
using static System.Activator;
using static System.String;

namespace FilesExplorerInDB_Manager.Implements
{
    public class FilesDbManager : IFilesDbManager
    {
        private IFilesDbService DBService { get; } = GetServer<IFilesDbService>();
        private IFileIcon FileIcon { get; } = GetServer<IFileIcon>();
        private IMonitorManager MonitorManager { get; } = GetServer<IMonitorManager>();
        private ExplorerProperty Property { get; set; }

        #region 基础操作

        public Files FilesAdd(FileInfo fileInfo, string folderLocalId, string pathForSave)
        {
            if (fileInfo == null) throw new Exception(Message_ArgumentNullException_FileInfo);
            if (!Directory.Exists(pathForSave)) Directory.CreateDirectory(pathForSave);

            Files files = (Files) CreateInstance(typeof(Files));
            files.FolderLocalId = folderLocalId;
            files.AccessTime = fileInfo.LastAccessTime;
            files.CreationTime = fileInfo.CreationTime;
            files.FileName = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf(".", StringComparison.Ordinal));
            files.FileType = fileInfo.Extension.Substring(1, fileInfo.Extension.Length - 1);
            files.ModifyTime = fileInfo.LastWriteTime;
            files.Size = fileInfo.Length;
            files.FileId = Guid.NewGuid().ToString();
            var originName = fileInfo.FullName;
            fileInfo = fileInfo.CopyTo(pathForSave + "\\" + files.FileId + "." + files.FileType, true);
            files.RealName = fileInfo.FullName;
            files = FilesAdd(files, true);
            MonitorManager.AddFileRecord(originName, files);
            return files;
        }

        private Files FilesAdd(Files entity, bool isSave, bool autoId = false)
        {
            entity = DBService.FilesAdd(entity, autoId);
            if (isSave) SaveChanges();
            return entity;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return DBService.FilesFind(keyValue);
        }

        public List<Files> LoadFilesEntities(Expression<Func<Files, bool>> where)
        {
            return DBService.LoadFilesEntities(where);
        }

        private Folders FoldersAdd(Folders entity, bool isSave, bool autoId = false)
        {
            entity = DBService.FoldersAdd(entity, autoId);
            if (isSave) SaveChanges();
            return entity;
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return DBService.FoldersFind(keyValue);
        }

        public List<Folders> LoadFoldersEntities(Expression<Func<Folders, bool>> where)
        {
            return DBService.LoadFoldersEntities(where);
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

        public List<ExplorerProperty> SetExplorerItemsList(string localFolderId, out Folders folderNow)
        {
            if (IsNullOrWhiteSpace(localFolderId) || localFolderId == App_RootLocalFolderId)
                localFolderId = GetSetting(SettingType.RootFolderId).ToString();

            folderNow = FoldersFind(localFolderId);

            if (folderNow == null) throw new Exception(Message_ResultIsNull_Folders);
            Debug.WriteLine(folderNow);
            folderNow.FolderNodes = LoadFoldersEntities(f => f.FolderLocalId == localFolderId && f.IsDelete == false);
            var list = folderNow.FolderNodes.Select(folder => SetExplorerItems_Folders(folder, GetImage(Resource.folder))).ToList();
            //TODO 获取图标的操作过于频繁
            list.AddRange(from file in folderNow.Files where !file.IsDelete select SetExplorerItems_Files(file));

            return list;
        }

        public List<ExplorerProperty> SetTrashItemsList()
        {
            var foldersTrashList = LoadFoldersEntities(f => f.IsDelete);
            var filesTrashList = LoadFilesEntities(f => f.IsDelete);
            var list = foldersTrashList.Select(folder => SetExplorerItems_Folders(folder, GetImage(Resource.folder), true))
                .ToList();
            //TODO 获取图标的操作过于频繁
            list.AddRange(from file in filesTrashList select SetExplorerItems_Files(file, true));

            return list;
        }

        public ExplorerProperty SetExplorerItem(Folders folderNow)
        {
            if (folderNow == null) throw new Exception(Message_ArgumentNullException_Folders);
            Debug.WriteLine(folderNow);
            var e = SetExplorerItems_Folders(folderNow, GetImage(folder));
            return e;
        }

        /// <summary>
        /// 设置文件信息
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="forTrash"></param>
        /// <returns>文件信息</returns>
        //// <param name="defaultBitmap">默认的文件类型图标</param>
        private ExplorerProperty SetExplorerItems_Files(Files file, bool forTrash = false)
        {
            Property = (ExplorerProperty) CreateInstance(typeof(ExplorerProperty));
            Property.Id = file.FileId;
            if (!forTrash) Property.FolderLocalId = file.FolderLocalId;
            Property.IsFolder = false;
            Property.Name = file.FileName;
            Property.AccessTime = file.AccessTime.ToString(CultureInfo.CurrentCulture);
            Property.CreationTime = file.CreationTime.ToString(CultureInfo.CurrentCulture);
            Property.ModifyTime = file.ModifyTime.ToString(CultureInfo.CurrentCulture);
            Property.Size = file.Size;
            Property.Type = file.FileType;
            //TODO 获取图标的操作过于频繁
            if (CheckFilePath(file))
            {
                Property.ImageSource =
                    GetImage(FileIcon.GetBitmapFromFilePath(file.RealName, ExtraLargeIcon));
            }
            else
            {
                //SetFilesProperty(file.FileId);
                Property.ImageSource = GetImage(fileNotFount);
            }
            if (!forTrash) return Property;

            var stack = GetRelativePath_Folder(file.FolderLocalId);
            var pathString = stack.Aggregate("", (current, f) => current + (f.FolderName + "/"));
            Property.OriginSite = pathString;
            return Property;
        }

        /// <summary>
        /// 设置文件夹信息
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="imageSource">文件夹图标</param>
        /// <param name="forTrash"></param>
        /// <returns>文件夹信息</returns>
        private ExplorerProperty SetExplorerItems_Folders(Folders folder, ImageSource imageSource,
            bool forTrash = false)
        {
            Property = (ExplorerProperty) CreateInstance(typeof(ExplorerProperty));
            Property.Id = folder.FolderId;
            if (!forTrash) Property.FolderLocalId = folder.FolderLocalId;
            Property.IsFolder = true;
            Property.Name = folder.FolderName;
            Property.AccessTime = "";
            Property.CreationTime = folder.CreationTime.ToString("yyyy/MM/dd HH:mm", CultureInfo.CurrentCulture);
            Property.ModifyTime = folder.ModifyTime.ToString("yyyy/MM/dd HH:mm", CultureInfo.CurrentCulture);
            Property.Size = folder.Size;
            Property.Type = Property_Type_Folder;
            Property.ImageSource = imageSource;
            if (!forTrash) return Property;

            var stack = GetRelativePath_Folder(folder.FolderId);
            var pathString = stack.Aggregate("", (current, f) => current + (f.FolderName + "/"));
            Property.OriginSite = pathString;
            return Property;
        }

        /// <summary>
        /// 通过递归的方式设置目录及其子目录
        /// </summary>
        /// <param name="folderId">父目录ID</param>
        /// <param name="folders">除了父目录外的其他目录数据</param>
        /// <returns>父目录数据</returns>
        public List<Folders> GetFoldersTree(string folderId, List<Folders> folders)
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

        #region 还原回收站中的项目

        /// <summary>
        /// 还原回收站中的的项目
        /// </summary>
        /// <param name="items">要还原的项目</param>
        /// <returns>成功状态</returns>
        public bool Restore(List<ExplorerProperty> items)
        {
            if (items == null) throw new Exception(Message_ArgumentNullException_ExplorerPropertyList);
            foreach (var item in items)
            {
                if (item.IsFolder)
                {
                    Folders folder = FoldersFind(item.Id);
                    if (folder == null) continue;
                    folder.IsDelete = false;
                    FoldersModified(folder);
                    MonitorManager.RestoreFolderRecord(folder);
                }
                else
                {
                    Files file = FilesFind(item.Id);
                    if (file == null) continue;
                    file.IsDelete = false;
                    FilesModified(file);
                    MonitorManager.RestoreFileRecord(file);
                }
            }

            return SaveChanges() > 0;
        }

        #endregion

        #region 搜索

        public List<ExplorerProperty> SearchResultList(string name, Folders folderNow)
        {
            if (IsNullOrWhiteSpace(name)) return null;

            var sFolder = LoadFoldersEntities(f =>
                f.FolderLocalId == folderNow.FolderId && f.FolderName.Contains(name) && !f.IsDelete);
            var cFolder = LoadFoldersEntities(f => f.FolderLocalId == folderNow.FolderId && !f.IsDelete);
            var list = sFolder.Select(folder => SetExplorerItems_Folders(folder, GetImage(Resource.folder))).ToList();
            //TODO 获取图标的操作过于频繁
            list.AddRange(from file in folderNow.Files
                where !file.IsDelete && file.FileName.Contains(name)
                select SetExplorerItems_Files(file));

            cFolder.ForEach(f => { list.AddRange(SearchResultList(name, f)); });
            return list;
        }

        #endregion

        #region 粘贴

        /// <summary>
        /// 文件、文件夹粘贴
        /// </summary>
        /// <param name="folderForPaste">粘贴到所在文件夹的ID</param>
        /// <param name="items">要粘贴的项目</param>
        /// <param name="isCutting">是否为剪切</param>
        /// <returns>是否成功</returns>
        public bool Paste(string folderForPaste, List<ExplorerProperty> items, bool isCutting)
        {
            if (items == null) throw new Exception(Message_ArgumentNullException_ExplorerPropertyList);
            var lid = "";
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
                            if (idForCheck == App_RootLocalFolderId || IsNullOrWhiteSpace(idForCheck)) break;
                            var f = FoldersFind(idForCheck);
                            if (f.FolderId == item.Id) throw new Exception(Message_PasteError);
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
                        PasteChild(folderForPaste, folder);
                        MonitorManager.CopyFolderRecord(folder, folderForPaste.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        Files file = FilesFind(item.Id);
                        Files file2 = (Files) CreateInstance(typeof(Files));
                        var oldLocalId = file.FolderLocalId;
                        file2.AccessTime = file.AccessTime;
                        file2.FileName = file.FileName;
                        file2.FileType = file.FileType;
                        file2.FolderLocalId = file.FolderLocalId;
                        file2.IsDelete = file.IsDelete;
                        file2.IsMiss = file.IsMiss;
                        file2.ModifyTime = file.ModifyTime;
                        file2.RealName = file.RealName;
                        file2.Size = file.Size;
                        file2.FolderLocalId = folderForPaste;
                        file2.CreationTime = DateTime.Now;
                        FilesAdd(file2, false, true);
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
        /// <param name="folder">原文件夹</param>
        private void PasteChild(string folderForPaste, Folders folder)
        {
            //DbManager = new FilesDbManager();

            //var folder = FoldersFind(folderId); //原文件夹
            var folder2 = (Folders) CreateInstance(folder.GetType());
            folder2.FolderLocalId = folderForPaste;
            folder2.CreationTime = DateTime.Now;
            folder2.FileIncludeCount = folder.FileIncludeCount;
            folder2.FolderIncludeCount = folder.FolderIncludeCount;
            folder2.FolderName = folder.FolderName;
            folder2.IsDelete = folder.IsDelete;
            folder2.ModifyTime = DateTime.Now;
            folder2.Size = folder.Size;
            folder2 = FoldersAdd(folder2, false, true);
            var foldersList = LoadFoldersEntities(f => f.FolderLocalId == folder.FolderId && !f.IsDelete);
            foreach (var folders in foldersList) //获取原文件夹的子文件夹
            {
                PasteChild(folder2.FolderId, folders);
            }

            var filesList = LoadFilesEntities(f => f.FolderLocalId == folder.FolderId && !f.IsDelete);
            foreach (var files in filesList) //获取原文件夹下的文件
            {
                var file2 = (Files)CreateInstance(typeof(Files));
                file2.AccessTime = files.AccessTime;
                file2.FileName = files.FileName;
                file2.FileType = files.FileType;
                file2.FolderLocalId = files.FolderLocalId;
                file2.IsDelete = files.IsDelete;
                file2.IsMiss = files.IsMiss;
                file2.ModifyTime = files.ModifyTime;
                file2.RealName = files.RealName;
                file2.Size = files.Size;
                file2.FolderLocalId = folder2.FolderId;
                file2.CreationTime = DateTime.Now;
                FilesAdd(file2, false, true);
            }
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
            if (items == null) throw new Exception(Message_ArgumentNullException_ExplorerPropertyList);
            var lid = "";
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
        /// 完全删除项目
        /// </summary>
        /// <param name="items">要删除的项目</param>
        /// <returns>成功状态</returns>
        public bool CompleteDelete(List<ExplorerProperty> items)
        {
            if (items == null) throw new Exception(Message_ArgumentNullException_ExplorerPropertyList);
            foreach (var item in items)
            {
                if (item.IsFolder)
                {
                    Folders folder = FoldersFind(item.Id);
                    if (folder == null) continue;
                    DBService.FoldersRemove(folder);
                    MonitorManager.DeleteFolderCompleteRecord(folder);
                }
                else
                {
                    Files file = FilesFind(item.Id);
                    if (file == null) continue;
                    DBService.FilesRemove(file);
                    MonitorManager.DeleteFileCompleteRecord(file);
                }
            }

            return SaveChanges() > 0;
        }

        /// <summary>
        /// 将项目标记为删除状态
        /// </summary>
        /// <param name="filesForDelete">要删除的文件的ID</param>
        /// <returns>成功状态</returns>
        public bool SetDeleteState(string filesForDelete)
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
            if (items == null) throw new Exception(Message_ArgumentNullException_ExplorerPropertyList);
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
            if (file == null) throw new Exception(Message_ArgumentNullException_Files);
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
            if (folder == null) throw new Exception(Message_ArgumentNullException_Folders);
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
        public Folders CreateFolders(string parentFoldersId)
        {
            Folders folder = GetServer<Folders>();
            folder.FolderLocalId = parentFoldersId;
            folder.CreationTime = DateTime.Now;
            folder.FileIncludeCount = 0;
            folder.FolderIncludeCount = 0;
            folder.FolderName = "新建文件夹" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff", CultureInfo.CurrentCulture);
            folder.IsDelete = false;
            folder.ModifyTime = DateTime.Now;
            folder.Size = 0;
            folder = FoldersAdd(folder, true, true);
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
        public Folders SetFoldersProperty(string foldersId)
        {
            Folders parentFolders = FoldersFind(foldersId);
            //将需要叠加统计的数据，初始化为0，否则将导致下一次叠加统计时数据错误。
            parentFolders.FileIncludeCount = 0;
            parentFolders.FolderIncludeCount = 0;
            parentFolders.Size = 0;
            parentFolders = SetChildFoldersProperty(parentFolders, App_RootLocalFolderId);
            SaveChanges();
            return parentFolders;
        }

        /// <summary>
        /// 刷新子文件夹的属性
        /// </summary>
        /// <param name="parentFolders">父文件夹</param>
        /// <param name="endFolderId">最终的文件夹（当设置子文件夹到最终文件夹时，停止继续设置子文件夹，避免设置整个文件夹系统）</param>
        /// <returns>子文件夹的属性</returns>
        private Folders SetChildFoldersProperty(Folders parentFolders, string endFolderId)
        {
            var folders = LoadFoldersEntities(f => f.FolderLocalId == parentFolders.FolderId && !f.IsDelete);
            var foldersList = new List<Folders>();
            if (parentFolders.FolderId != endFolderId) //当刷新子文件夹到最终文件夹时，停止继续刷新子文件夹
                foreach (var childFolders in folders)
                {
                    //将需要叠加统计的数据，初始化为0，否则将导致下一次叠加统计时数据错误。
                    childFolders.FileIncludeCount = 0;
                    childFolders.FolderIncludeCount = 0;
                    childFolders.Size = 0;
                    foldersList.Add(SetChildFoldersProperty(childFolders, endFolderId));
                }

            var files = LoadFilesEntities(f => f.FolderLocalId == parentFolders.FolderId && !f.IsDelete);
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
        private Folders SetParentFoldersProperty(string foldersId)
        {
            if (foldersId == App_RootLocalFolderId || IsNullOrWhiteSpace(foldersId))
            {
                SaveChanges();
                return null;
            }

            var folders = LoadFoldersEntities(f => f.FolderLocalId == foldersId && !f.IsDelete);
            var files = LoadFilesEntities(f => f.FolderLocalId == foldersId && !f.IsDelete);
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
            return filesList.Select(files => SetFilesProperty(files.FileId)).ToList();
        }

        /// <summary>
        /// 刷新文件属性
        /// </summary>
        /// <param name="filesId">文件ID</param>
        /// <returns>文件属性</returns>
        public Files SetFilesProperty(string filesId)
        {
            Files files = FilesFind(filesId);
            if (files != null && !IsNullOrWhiteSpace(files.RealName))
            {
                FileInfo fileInfo = new FileInfo(files.RealName);
                if (fileInfo.Exists && CheckFilePath(files))
                {
                    files.IsMiss = false;
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

        public Stack<Folders> GetRelativePath_Folder(string folderId)
        {
            var stack = new Stack<Folders>();
            while (folderId != App_RootLocalFolderId)
            {
                Folders folders = FoldersFind(folderId);
                if (folders == null) break;
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

        private static bool CheckFilePath(Files files)
        {
            if (files == null || IsNullOrWhiteSpace(files.RealName)) return false;
            if (!File.Exists(files.RealName)) return false;
            var nameMatches = Regex.Matches(files.RealName,
                @"([\d|A-Za-z]+\." + files.FileType + @")|([\d+|A-Za-z\-\d|A-Za-z]+\." + files.FileType + ")");
            if (nameMatches.Count < 1)
            {
                Debug.WriteLine("CheckFilePath Error");
                return false;
            }

            var name = nameMatches[nameMatches.Count - 1];
            return File.Exists(GetSetting(SettingType.FileStorageLocation) + "\\" + name);
        }
    }
}