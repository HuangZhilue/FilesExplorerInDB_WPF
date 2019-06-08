using System;
using System.Linq;
using System.Linq.Expressions;
using FilesExplorerInDB_EF.EFModels;

namespace FilesExplorerInDB_MongoDb.Interface
{
    public interface IMonitorMongoDbService
    {
        Monitor MonitorAdd(Monitor entity);

        Monitor MonitorFind(params object[] keyValue);

        void MonitorModified(Monitor entity);

        void MonitorRemove(Monitor entity);

        IQueryable<Monitor> LoadMonitorEntites(Expression<Func<Monitor, bool>> where);

        int SaveChanges();
    }
}