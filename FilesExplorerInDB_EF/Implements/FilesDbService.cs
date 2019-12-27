using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;

namespace FilesExplorerInDB_EF.Implements
{
    public class FilesDbService : IFilesDbService
    {
        private readonly FilesDB _dbContext = FilesDB.GetFilesDb;

        public Files FilesAdd(Files entity)
        {
            _dbContext.Files.Add(entity);
            return entity;
        }

        public Folders FoldersAdd(Folders entity)
        {
            _dbContext.Folders.Add(entity);
            return entity;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return _dbContext.Files.Find(keyValue);
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return _dbContext.Folders.Find(keyValue);
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
            _dbContext.Files.Remove(entity);
        }

        public void FoldersRemove(Folders entity)
        {
            _dbContext.Folders.Remove(entity);
        }

        public List<Files> LoadFilesEntites(Expression<Func<Files, bool>> where)
        {
            return _dbContext.Files.Where(where).ToList();
        }

        public List<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> where)
        {
            return _dbContext.Folders.Where(where).ToList();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}