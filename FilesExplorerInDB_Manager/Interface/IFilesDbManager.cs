﻿using FilesExplorerInDB_EF.EFModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Media;
using FilesExplorerInDB_Models.Models;

namespace FilesExplorerInDB_Manager.Interface
{
    public interface IFilesDbManager
    {
        Files FilesAdd(FileInfo fileInfo, int folderLocalId, string pathForSave);

        Files FilesFind(params object[] keyValue);

        Folders FoldersFind(params object[] keyValue);

        IQueryable<Files> LoadFilesEntites(Expression<Func<Files, bool>> where);

        IQueryable<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> where);

        int SaveChanges();

        ExplorerProperty SetExplorerItems_Files(Files file, ImageSource imageSource);

        ExplorerProperty SetExplorerItems_Folders(Folders file, ImageSource imageSource);

        List<Folders> GetFoldersTree(int folderId, List<Folders> folders);

        bool Paste(int folderForPaste, List<ExplorerProperty> items, bool isCutting);

        bool SetDeleteState(List<ExplorerProperty> items);

        bool SetDeleteState(int filesForDelete);

        bool Rename(List<ExplorerProperty> items, string newName);

        Stack<Folders> GetRelativePath_Folder(int folderId);

        Folders CreateFolders(int ParentFoldersId);

        Folders SetFoldersProperty(int foldersId);

        Files SetFilesProperty(int filesId);
    }
}
