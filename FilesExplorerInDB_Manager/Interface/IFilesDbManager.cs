using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Models.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Media;

namespace FilesExplorerInDB_Manager.Interface
{
    public interface IFilesDbManager
    {
        Files FilesAdd(FileInfo fileInfo, string folderLocalId, string pathForSave);

        Files FilesFind(params object[] keyValue);

        Folders FoldersFind(params object[] keyValue);

        List<Files> LoadFilesEntities(Expression<Func<Files, bool>> where);

        List<Folders> LoadFoldersEntities(Expression<Func<Folders, bool>> where);

        ExplorerProperty SetExplorerItem(Folders folderNow);

        List<ExplorerProperty> SetExplorerItemsList(string localFolderId, out Folders folderNow);

        List<Folders> GetFoldersTree(string folderId, List<Folders> folders);

        bool Paste(string folderForPaste, List<ExplorerProperty> items, bool isCutting);

        bool SetDeleteState(List<ExplorerProperty> items);

        bool SetDeleteState(string filesForDelete);

        bool Rename(List<ExplorerProperty> items, string newName);

        void Rename(Files file, string newName);

        void Rename(Folders folder, string newName);

        Stack<Folders> GetRelativePath_Folder(string folderId);

        Folders CreateFolders(string parentFoldersId);

        Folders SetFoldersProperty(string foldersId);

        Files SetFilesProperty(string filesId);

        ImageSource GetImage(Bitmap imageBitmap);

        string DisplayFileSize(long size);
    }
}