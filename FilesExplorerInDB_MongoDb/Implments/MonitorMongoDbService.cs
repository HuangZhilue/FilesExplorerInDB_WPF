using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using Sikiro.Nosql.Mongo;

namespace FilesExplorerInDB_MongoDb.Implments
{
    public class MonitorMongoDbService : IMonitorService
    {
        private readonly MongoRepository _mongoRepository = new MongoRepository(ConfigurationManager.ConnectionStrings["FilesDB_MongoDB"].ConnectionString);

        public Monitor MonitorAdd(Monitor entity)
        {
            entity.MonitorId = _mongoRepository.Count<Monitor>(f => f.MonitorId != -1) + 1;
            return _mongoRepository.Add(entity) ? entity : null;
        }

        public Monitor MonitorFind(params object[] keyValue)
        {
            return _mongoRepository.Get<Monitor>(f => f.MonitorId == Convert.ToInt32(keyValue[0]));
        }

        public void MonitorModified(Monitor entity)
        {
            _mongoRepository.Update(entity);
        }

        public void MonitorRemove(Monitor entity)
        {
            _mongoRepository.Delete<Monitor>(f => f.MonitorId == entity.MonitorId);
        }

        public IQueryable<Monitor> LoadMonitorEntites(Expression<Func<Monitor, bool>> where)
        {
            return _mongoRepository.ToList(where).AsQueryable();
        }

        public int SaveChanges()
        {
            return 1;
        }
    }
}