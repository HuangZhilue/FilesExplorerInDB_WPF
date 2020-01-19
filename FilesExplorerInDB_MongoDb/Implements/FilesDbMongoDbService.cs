using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using Sikiro.Nosql.Mongo;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static Resources.Resource;
using static System.Configuration.ConfigurationManager;

namespace FilesExplorerInDB_MongoDb.Implements
{
    public class FilesDbMongoDbService : IFilesDbService
    {
        private MongoRepository MongoRepository { get; } =
            new MongoRepository(ConnectionStrings["FilesDB_MongoDB"].ConnectionString);

        public Files FilesAdd(Files entity, bool autoId = false)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Files);
            if (autoId) entity.FileId = Guid.NewGuid().ToString();
            MongoRepository.Add(entity);
            return entity;
        }

        public Folders FoldersAdd(Folders entity, bool autoId = false)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Folders);
            if (autoId) entity.FolderId = Guid.NewGuid().ToString();
            MongoRepository.Add(entity);
            return entity;
        }

        public Files FilesFind(params object[] keyValue)
        {
            return MongoRepository.Get<Files>(f => f.FileId == keyValue[0].ToString());
        }

        public Folders FoldersFind(params object[] keyValue)
        {
            return MongoRepository.Get<Folders>(f => f.FolderId == keyValue[0].ToString());
        }

        public void FilesModified(Files entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Files);
            MongoRepository.Update(entity);
        }

        public void FoldersModified(Folders entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Folders);
            MongoRepository.Update(entity);
        }

        public void FilesRemove(Files entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Files);
            MongoRepository.Delete<Files>(f => f.FileId == entity.FileId);
        }

        public void FoldersRemove(Folders entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Folders);
            MongoRepository.Delete<Folders>(f => f.FolderId == entity.FolderId);
        }

        public List<Files> LoadFilesEntities(Expression<Func<Files, bool>> where)
        {
            return MongoRepository.ToList(where);
        }

        public List<Folders> LoadFoldersEntities(Expression<Func<Folders, bool>> where)
        {
            return MongoRepository.ToList(where);
        }

        public int SaveChanges()
        {
            return 1;
        }
    }
}