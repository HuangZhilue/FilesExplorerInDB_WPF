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
    public class MonitorMongoDbService : IMonitorService
    {
        private MongoRepository MongoRepository { get; } =
            new MongoRepository(ConnectionStrings["FilesDB_MongoDB"].ConnectionString);

        public Monitor MonitorAdd(Monitor entity, bool autoId = false)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Monitor);
            if (autoId) entity.MonitorId = Guid.NewGuid().ToString();
            MongoRepository.Add(entity);
            return entity;
        }

        public Monitor MonitorFind(params object[] keyValue)
        {
            return MongoRepository.Get<Monitor>(f => f.MonitorId == keyValue[0].ToString());
        }

        public void MonitorModified(Monitor entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Monitor);
            MongoRepository.Update(entity);
        }

        public void MonitorRemove(Monitor entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Monitor);
            MongoRepository.Delete<Monitor>(f => f.MonitorId == entity.MonitorId);
        }

        public List<Monitor> LoadMonitorEntities(Expression<Func<Monitor, bool>> where)
        {
            return MongoRepository.ToList(where);
        }

        public int SaveChanges()
        {
            return 1;
        }
    }
}