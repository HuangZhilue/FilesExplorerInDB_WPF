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
    public class MonitorMongoDbService : IMonitorService
    {
        private readonly MongoRepository _mongoRepository = new MongoRepository(ConfigurationManager.ConnectionStrings["FilesDB_MongoDB"].ConnectionString);

        public Monitor MonitorAdd(Monitor entity)
        {
            if (entity == null) throw new Exception(Resource.Message_ArgumentNullException_Monitor);
            entity.MonitorId = (int)_mongoRepository.Count<Monitor>(f => f.MonitorId != -1) + 1;
            _mongoRepository.Add(entity);
            return entity;
        }

        public Monitor MonitorFind(params object[] keyValue)
        {
            return _mongoRepository.Get<Monitor>(f => f.MonitorId == Convert.ToInt32(keyValue[0],CultureInfo.CurrentCulture));
        }

        public void MonitorModified(Monitor entity)
        {
            _mongoRepository.Update(entity);
        }

        public void MonitorRemove(Monitor entity)
        {
            _mongoRepository.Delete<Monitor>(f => f.MonitorId == entity.MonitorId);
        }

        public List<Monitor> LoadMonitorEntites(Expression<Func<Monitor, bool>> where)
        {
            return _mongoRepository.ToList(where);
        }

        public int SaveChanges()
        {
            return 1;
        }
    }
}