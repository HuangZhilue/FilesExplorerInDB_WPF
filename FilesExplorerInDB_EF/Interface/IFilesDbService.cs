using FilesExplorerInDB_EF.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FilesExplorerInDB_EF.Interface
{
    public interface IFilesDbService
    {
        Files FilesAdd(Files entity);

        Folders FoldersAdd(Folders entity);

        Files FilesFind(params object[] keyValue);

        Folders FoldersFind(params object[] keyValue);

        void FilesModified(Files entity);

        void FoldersModified(Folders entity);

        void FilesRemove(Files entity);

        void FoldersRemove(Folders entity);

        List<Files> LoadFilesEntites(Expression<Func<Files, bool>> @where);

        List<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> @where);

        int SaveChanges();
    }
}
