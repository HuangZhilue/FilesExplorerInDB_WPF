using Command;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FilesExplorerInDB_EF.Implments
{
    public class FilesDbService : IFilesDbService
    {
        private readonly FilesDB _dbContext = UnityContainerHelp.GetServer<FilesDB>();

        public Files FilesAdd(Files entity)
        {
            _dbContext.Set<Files>().Add(entity);
            return entity;
        }

        public Folders FoldersAdd(Folders entity)
        {
            _dbContext.Set<Folders>().Add(entity);
            return entity;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return _dbContext.Set<Files>().Find(keyValue);
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return _dbContext.Set<Folders>().Find(keyValue);
        }

        public void FilesModified(Files entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void FoldersModified(Folders entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void FilesRemove(Files entity)
        {
            _dbContext.Set<Files>().Remove(entity);
        }

        public void FoldersRemove(Folders entity)
        {
            _dbContext.Set<Folders>().Remove(entity);
        }

        public IQueryable<Files> LoadFilesEntites(Expression<Func<Files, bool>> where)
        {
            return _dbContext.Set<Files>().Where(where);
        }

        public IQueryable<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> where)
        {
            return _dbContext.Set<Folders>().Where(where);
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}