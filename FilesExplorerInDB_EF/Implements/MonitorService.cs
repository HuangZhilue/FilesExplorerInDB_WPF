using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;

namespace FilesExplorerInDB_EF.Implements
{
    public class MonitorService : IMonitorService
    {
        //private readonly FilesDB _dbContext = FilesDB.GetFilesDb;
        private readonly FilesDB _dbContext = (FilesDB) Activator.CreateInstance(typeof(FilesDB));

        public Monitor MonitorAdd(Monitor entity)
        {
            _dbContext.Monitor.Add(entity);
            return entity;
        }

        public Monitor MonitorFind(params object[] keyValue)
        {
            return _dbContext.Monitor.Find(keyValue);
        }

        public void MonitorModified(Monitor entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void MonitorRemove(Monitor entity)
        {
            _dbContext.Monitor.Remove(entity);
        }

        public List<Monitor> LoadMonitorEntites(Expression<Func<Monitor, bool>> @where)
        {
            return _dbContext.Monitor.Where(where).ToList();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}