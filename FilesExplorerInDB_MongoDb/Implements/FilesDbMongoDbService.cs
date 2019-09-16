using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using Sikiro.Nosql.Mongo;

namespace FilesExplorerInDB_MongoDb.Implements
{
    public class FilesDbMongoDbService : IFilesDbService
    {
        private readonly MongoRepository _mongoRepository = new MongoRepository(ConfigurationManager.ConnectionStrings["FilesDB_MongoDB"].ConnectionString);

        public Files FilesAdd(Files entity)
        {
            entity.FileId = _mongoRepository.Count<Files>(f => f.FileId != -1) + 1;
            return _mongoRepository.Add(entity) ? entity : null;
        }

        public Folders FoldersAdd(Folders entity)
        {
            entity.FolderId = _mongoRepository.Count<Folders>(f => f.FolderId != -1) + 1;
            return _mongoRepository.Add(entity) ? entity : null;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return _mongoRepository.Get<Files>(f => f.FileId == Convert.ToInt32(keyValue[0]));
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return _mongoRepository.Get<Folders>(f => f.FolderId == Convert.ToInt32(keyValue[0]));
        }

        public void FilesModified(Files entity)
        {
            _mongoRepository.Update(entity);
        }

        public void FoldersModified(Folders entity)
        {
            _mongoRepository.Update(entity);
        }

        public void FilesRemove(Files entity)
        {
            _mongoRepository.Delete<Files>(f => f.FileId == entity.FileId);
        }

        public void FoldersRemove(Folders entity)
        {
            _mongoRepository.Delete<Folders>(f => f.FolderId == entity.FolderId);
        }

        public IQueryable<Files> LoadFilesEntites(Expression<Func<Files, bool>> where)
        {
            return _mongoRepository.ToList(where).AsQueryable();
        }

        public IQueryable<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> where)
        {
            return _mongoRepository.ToList(where).AsQueryable();
        }

        public int SaveChanges()
        {
            return 1; 
        }
    }
}
