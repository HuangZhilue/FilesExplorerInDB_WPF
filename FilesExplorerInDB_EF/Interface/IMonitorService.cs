using FilesExplorerInDB_EF.EFModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FilesExplorerInDB_EF.Interface
{
    public interface IMonitorService
    {
        Monitor MonitorAdd(Monitor entity, bool autoId = false);

        Monitor MonitorFind(params object[] keyValue);

        void MonitorModified(Monitor entity);

        void MonitorRemove(Monitor entity);

        List<Monitor> LoadMonitorEntities(Expression<Func<Monitor, bool>> @where);

        int SaveChanges();
    }
}