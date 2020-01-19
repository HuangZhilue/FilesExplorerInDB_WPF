using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using static Resources.Resource;
using static System.Activator;

namespace FilesExplorerInDB_EF.Implements
{
    public class MonitorService : IMonitorService
    {
        private FilesDB DBContext { get; } = (FilesDB) CreateInstance(typeof(FilesDB));

        public Monitor MonitorAdd(Monitor entity, bool autoId = false)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Monitor);
            if (autoId) entity.MonitorId = Guid.NewGuid().ToString();
            DBContext.Monitor.Add(entity);
            return entity;
        }

        public Monitor MonitorFind(params object[] keyValue)
        {
            return DBContext.Monitor.Find(keyValue);
        }

        public void MonitorModified(Monitor entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Monitor);
            DBContext.Entry(entity).State = EntityState.Modified;
        }

        public void MonitorRemove(Monitor entity)
        {
            if (entity == null) throw new Exception(Message_ArgumentNullException_Monitor);
            DBContext.Monitor.Remove(entity);
        }

        public List<Monitor> LoadMonitorEntities(Expression<Func<Monitor, bool>> @where)
        {
            return DBContext.Monitor.Where(where).ToList();
        }

        public int SaveChanges()
        {
            return DBContext.SaveChanges();
        }
    }
}