using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using static FilesExplorerInDB_EF.EFModels.FilesDB;
using static Resources.Resource;

namespace FilesExplorerInDB_EF.Implements
{
    public class FilesDbService : IFilesDbService
    {
        private FilesDB DBContext { get; } = GetFilesDb;

        public Files FilesAdd(Files entity, bool autoId = false)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Files);
            if (autoId) entity.FileId = Guid.NewGuid().ToString();
            DBContext.Files.Add(entity);
            return entity;
        }

        public Folders FoldersAdd(Folders entity, bool autoId = false)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Folders);
            if (autoId) entity.FolderId = Guid.NewGuid().ToString();
            DBContext.Folders.Add(entity);
            return entity;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return DBContext.Files.Find(keyValue);
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return DBContext.Folders.Find(keyValue);
        }

        public void FilesModified(Files entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Files);
            DBContext.Entry(entity).State = EntityState.Modified;
        }

        public void FoldersModified(Folders entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Folders);
            DBContext.Entry(entity).State = EntityState.Modified;
        }

        public void FilesRemove(Files entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Files);
            DBContext.Files.Remove(entity);
        }

        public void FoldersRemove(Folders entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Folders);
            DBContext.Folders.Remove(entity);
        }

        public List<Files> LoadFilesEntities(Expression<Func<Files, bool>> where)
        {
            return DBContext.Files.Where(where).ToList();
        }

        public List<Folders> LoadFoldersEntities(Expression<Func<Folders, bool>> where)
        {
            return DBContext.Folders.Where(where).ToList();
        }

        public int SaveChanges()
        {
            return DBContext.SaveChanges();
        }
    }
}