using FilesExplorerInDB_EF.EFModels;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        List<Files> LoadFilesEntites(Expression<Func<Files, bool>> where);

        List<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> where);

        ExplorerProperty SetExplorerItem(Folders folderNow);

        List<ExplorerProperty> SetExplorerItemsList(int localFolderId, out Folders folderNow);

        //ExplorerProperty SetExplorerItems_Files(Files file, Bitmap defaultBitmap, Bitmap errorBitmap);

        //ExplorerProperty SetExplorerItems_Folders(Folders file, ImageSource imageSource);

        List<Folders> GetFoldersTree(int folderId, List<Folders> folders);

        bool Paste(int folderForPaste, List<ExplorerProperty> items, bool isCutting);

        bool SetDeleteState(List<ExplorerProperty> items);

        bool SetDeleteState(int filesForDelete);

        bool Rename(List<ExplorerProperty> items, string newName);

        void Rename(Files file, string newName);

        void Rename(Folders folder, string newName);

        Stack<Folders> GetRelativePath_Folder(int folderId);

        Folders CreateFolders(int parentFoldersId);

        Folders SetFoldersProperty(int foldersId);

        Files SetFilesProperty(int filesId);

        ImageSource GetImage(Bitmap imageBitmap);

        string DisplayFileSize(long size);
    }
}