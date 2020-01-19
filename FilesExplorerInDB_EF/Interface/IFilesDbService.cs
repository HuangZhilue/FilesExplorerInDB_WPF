using FilesExplorerInDB_EF.EFModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FilesExplorerInDB_EF.Interface
{
    public interface IFilesDbService
    {
        Files FilesAdd(Files entity, bool autoId = false);

        Folders FoldersAdd(Folders entity, bool autoId = false);

        Files FilesFind(params object[] keyValue);

        Folders FoldersFind(params object[] keyValue);

        void FilesModified(Files entity);

        void FoldersModified(Folders entity);

        void FilesRemove(Files entity);

        void FoldersRemove(Folders entity);

        List<Files> LoadFilesEntities(Expression<Func<Files, bool>> where);

        List<Folders> LoadFoldersEntities(Expression<Func<Folders, bool>> where);

        int SaveChanges();
    }
}
