using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Command;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;

namespace FilesExplorerInDB_EF.Implements
{
    public class MonitorService : IMonitorService
    {
        private readonly FilesDB _dbContext = UnityContainerHelp.GetServer<FilesDB>();

        public Monitor MonitorAdd(Monitor entity)
        {
            _dbContext.Set<Monitor>().Add(entity);
            return entity;
        }

        public Monitor MonitorFind(params object[] keyValue)
        {
            return _dbContext.Set<Monitor>().Find(keyValue);
        }

        public void MonitorModified(Monitor entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void MonitorRemove(Monitor entity)
        {
            _dbContext.Set<Monitor>().Remove(entity);
        }

        public IQueryable<Monitor> LoadMonitorEntites(Expression<Func<Monitor, bool>> where)
        {
            return _dbContext.Set<Monitor>().Where(where);
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}