using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using Resources;
using Sikiro.Nosql.Mongo;

namespace FilesExplorerInDB_MongoDb.Implements
{
    public class FilesDbMongoDbService : IFilesDbService
    {
        private readonly MongoRepository _mongoRepository = new MongoRepository(ConfigurationManager.ConnectionStrings["FilesDB_MongoDB"].ConnectionString);

        public Files FilesAdd(Files entity)
        {
            if (entity == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            entity.FileId = (int)_mongoRepository.Count<Files>(f => f.FileId != -1) + 1;
            _mongoRepository.Add(entity);
            return entity;
        }

        public Folders FoldersAdd(Folders entity)
        {
            if (entity == null) throw new Exception(Resource.Message_ArgumentNullException_Folders);
            entity.FolderId = (int)_mongoRepository.Count<Folders>(f => f.FolderId != -1) + 1;
            _mongoRepository.Add(entity);
            return entity;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return _mongoRepository.Get<Files>(
                f => f.FileId == Convert.ToInt32(keyValue[0], CultureInfo.CurrentCulture));
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return _mongoRepository.Get<Folders>(f =>
                f.FolderId == Convert.ToInt32(keyValue[0], CultureInfo.CurrentCulture));
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

        public List<Files> LoadFilesEntites(Expression<Func<Files, bool>> @where)
        {
            return _mongoRepository.ToList(where);
        }

        public List<Folders> LoadFoldersEntites(Expression<Func<Folders, bool>> @where)
        {
            return _mongoRepository.ToList(where);
        }

        public int SaveChanges()
        {
            return 1; 
        }
    }
}
