using Command;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using FilesExplorerInDB_Manager.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Media;
using FilesExplorerInDB_Models.Models;

namespace FilesExplorerInDB_Manager.Implments
{
    public class FilesDbManager : IFilesDbManager
    {
        private readonly IFilesDbService _dbService = UnityContainerHelp.GetServer<IFilesDbService>();
        private ExplorerProperty _property;
        private FilesDbManager _filesDbManager;

        public Files FilesAdd(FileInfo fileInfo, int folderLocalId, string pathForSave)
        {
            if (!Directory.Exists(pathForSave))
            {
                Directory.CreateDirectory(pathForSave);
            }
            Files files = UnityContainerHelp.GetServer<Files>();
            files.FolderLocalId = folderLocalId;
            files.AccessTime = fileInfo.LastAccessTime;
            files.CreationTime = fileInfo.CreationTime;
            files.FileName = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf(".", StringComparison.Ordinal));
            files.FileType = fileInfo.Extension.Substring(1, fileInfo.Extension.Length - 1);
            files.ModifyTime = fileInfo.LastWriteTime;
            files.Size = fileInfo.Length;
            files = FilesAdd(files, true);
            fileInfo = fileInfo.CopyTo(pathForSave + "\\" + files.FileId + "." + files.FileType, true);
            files.RealName = fileInfo.FullName;
            _dbService.FilesModified(files);
            SaveChanges();
            return files;
        }

        private Files FilesAdd(Files entity, bool isSave)
        {
            entity = _dbService.FilesAdd(entity);
            if (isSave)
                SaveChanges();
            return entity;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return _dbService.FilesFind(keyValue);
        }

        public IQueryable<Files> LoadFilesEntites(Expression<Func<Files, bool>> where)
        {
            return _dbService.LoadFilesEntites(where);
        }

        private Folders FoldersAdd(Folders entity, bool isSave)
        {
            entity = _dbService.FoldersAdd(entity);
            if (isSave)
                SaveChanges();
            return entity;
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return _dbService.FoldersFind(keyValue);
        }

        public IQueryable<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> where)
        {
            return _dbService.LoadFoldersEntites(where);
        }

        public int SaveChanges()
        {
            return _dbService.SaveChanges();
        }

        public ExplorerProperty SetExplorerItems_Files(Files file, ImageSource imageSource)
        {
            _property = UnityContainerHelp.GetServer<ExplorerProperty>();
            _property.Id = file.FileId;
            _property.FolderLocalId = file.FolderLocalId;
            _property.IsFolder = false;
            _property.Name = file.FileName;
            _property.AccessTime = file.AccessTime.ToString("yyyy/MM/dd HH:mm");
            _property.CreationTime = file.CreationTime.ToString("yyyy/MM/dd HH:mm");
            _property.ModifyTime = file.ModifyTime.ToString("yyyy/MM/dd HH:mm");
            _property.Size = file.Size;
            _property.Type = file.FileType;
            if (imageSource != null)
                _property.ImageSource = imageSource;
            return _property;
        }

        public ExplorerProperty SetExplorerItems_Folders(Folders folder,ImageSource imageSource)
        {
            _property = UnityContainerHelp.GetServer<ExplorerProperty>();
            _property.Id = folder.FolderId;
            _property.FolderLocalId = folder.FolderLocalId;
            _property.IsFolder = true;
            _property.Name = folder.FolderName;
            _property.AccessTime = "";
            _property.CreationTime = folder.CreationTime.ToString("yyyy/MM/dd HH:mm");
            _property.ModifyTime = folder.ModifyTime.ToString("yyyy/MM/dd HH:mm");
            _property.Size = folder.Size;
            _property.Type = "文件夹";
            _property.ImageSource = imageSource;
            return _property;
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
            foreach (ExplorerProperty item in items)
            {
                if (isCutting) //剪切
                {
                    if (item.IsFolder)
                    {
                        Folders folder = FoldersFind(item.Id);
                        folder.FolderLocalId = folderForPaste;
                        _dbService.FoldersModified(folder);
                    }
                    else
                    {
                        Files file = FilesFind(item.Id);
                        file.FolderLocalId = folderForPaste;
                        _dbService.FilesModified(file);
                    }
                }
                else //复制
                {
                    if (item.IsFolder)
                    {
                        Folders folder = FoldersFind(item.Id);
                        PasteChild(folderForPaste, folder.FolderId);
                    }
                    else
                    {
                        Files file = FilesFind(item.Id);
                        file.FolderLocalId = folderForPaste;
                        Files file2 = UnityContainerHelp.GetServer<Files>();
                        file2.FolderLocalId = folderForPaste;
                        file2.AccessTime = DateTime.Now;
                        file2.CreationTime = DateTime.Now;
                        file2.FileName = file.FileName;
                        file2.FileType = file.FileType;
                        file2.IsDelete = file.IsDelete;
                        file2.ModifyTime = DateTime.Now;
                        file2.Size = file.Size;
                        file2.RealName = file.RealName;
                        FilesAdd(file, false);
                    }
                }
            }

            return SaveChanges() > 0;
        }

        /// <summary>
        /// 粘贴（子文件夹与子文件）
        /// </summary>
        /// <param name="folderForPaste">要粘贴到的文件夹的ID</param>
        /// <param name="folderId">原文件夹的ID</param>
        private void PasteChild(int folderForPaste, int folderId)
        {
            _filesDbManager = new FilesDbManager();

            Folders folder = FoldersFind(folderId); //原文件夹
            Folders folder2 = UnityContainerHelp.GetServer<Folders>();
            folder2.FolderLocalId = folderForPaste;
            folder2.CreationTime = DateTime.Now;
            folder2.FileIncludeCount = folder.FileIncludeCount;
            folder2.FolderIncludeCount = folder.FolderIncludeCount;
            folder2.FolderName = folder.FolderName;
            folder2.IsDelete = folder.IsDelete;
            folder2.ModifyTime = DateTime.Now;
            folder2.Size = folder.Size;
            folder2 = _filesDbManager.FoldersAdd(folder2, true);
            IQueryable<Folders> foldersList = LoadFoldersEntites(f => f.FolderLocalId == folder.FolderId);
            foreach (var folders in foldersList) //获取原文件夹的子文件夹
            {
                PasteChild(folder2.FolderId, folders.FolderId);
            }

            IQueryable<Files> filesList = LoadFilesEntites(f => f.FolderLocalId == folder.FolderId);
            foreach (var files in filesList) //获取原文件夹下的文件
            {
                Files file = UnityContainerHelp.GetServer<Files>();
                file.FolderLocalId = folder2.FolderId;
                file.AccessTime = DateTime.Now;
                file.CreationTime = DateTime.Now;
                file.FileName = files.FileName;
                file.FileType = files.FileType;
                file.IsDelete = files.IsDelete;
                file.ModifyTime = DateTime.Now;
                file.Size = files.Size;
                file.RealName = files.RealName;
                _filesDbManager.FilesAdd(file, false);
            }

            _filesDbManager.SaveChanges();
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
            foreach (ExplorerProperty item in items)
            {
                if (item.IsFolder)
                {
                    Folders folder = FoldersFind(item.Id);
                    folder.IsDelete = true;
                    _dbService.FoldersModified(folder);
                }
                else
                {
                    Files file = FilesFind(item.Id);
                    file.IsDelete = true;
                    _dbService.FilesModified(file);
                }
            }

            return SaveChanges() > 0;
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
            _dbService.FilesModified(file);
            return SaveChanges() > 0;
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
            foreach (ExplorerProperty item in items)
            {
                if (item.IsFolder)
                {
                    Folders folder = FoldersFind(item.Id);
                    folder.FolderName = newName;
                    folder.ModifyTime = DateTime.Now;
                    _dbService.FoldersModified(folder);
                }
                else
                {
                    Files file = FilesFind(item.Id);
                    file.FileName = newName;
                    file.ModifyTime = DateTime.Now;
                    _dbService.FilesModified(file);
                }
            }

            return SaveChanges() > 0;
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
            folder.FolderName = "新建文件夹" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
            folder.IsDelete = false;
            folder.ModifyTime = DateTime.Now;
            folder.Size = 0;
            folder = FoldersAdd(folder, true);
            return folder;
        }

        #endregion

        #region 设置属性

        #region 设置文件夹属性

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
            IQueryable<Folders> folders =
                LoadFoldersEntites(f => f.FolderLocalId == parentFolders.FolderId && !f.IsDelete);
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

            IQueryable<Files> files = LoadFilesEntites(f => f.FolderLocalId == parentFolders.FolderId && !f.IsDelete);
            if (!folders.Any())
            {
                parentFolders.FileIncludeCount = files.Count();
                parentFolders.FolderIncludeCount = folders.Count();
                parentFolders.Size = files.Select(f => f.Size).Sum();
            }
            else
            {
                parentFolders.FileIncludeCount += files.Count();
                parentFolders.FolderIncludeCount += folders.Count();
                parentFolders.Size += files.Select(f => f.Size).Sum();
            }

            parentFolders.FileIncludeCount += foldersList.Select(f => f.FileIncludeCount).Sum();
            parentFolders.FolderIncludeCount += foldersList.Select(f => f.FolderIncludeCount).Sum();
            parentFolders.Size += foldersList.Select(f => f.Size).Sum();
            parentFolders.ModifyTime = DateTime.Now;
            _dbService.FoldersModified(parentFolders);
            return parentFolders;
        }

        #endregion

        #region 设置文件属性

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
                    _dbService.FilesModified(files);

                    Folders parentFolders = FoldersFind(0);
                    //将需要叠加统计的数据，初始化为0，否则将导致下一次叠加统计时数据错误。
                    parentFolders.FileIncludeCount = 0;
                    parentFolders.FolderIncludeCount = 0;
                    parentFolders.Size = 0;
                    SetChildFoldersProperty(parentFolders, files.FolderLocalId);

                    SaveChanges();

                    return files;
                }
            }

            return null;
        }

        #endregion

        #endregion

        #region 获取相对路径

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

        #region 获取文件的相对路径



        #endregion

        #endregion
    }
}
