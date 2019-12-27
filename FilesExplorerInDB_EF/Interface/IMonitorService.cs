using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FilesExplorerInDB_EF.EFModels;

namespace FilesExplorerInDB_EF.Interface
{
    public interface IMonitorService
    {
        Monitor MonitorAdd(Monitor entity);

        Monitor MonitorFind(params object[] keyValue);

        void MonitorModified(Monitor entity);

        void MonitorRemove(Monitor entity);

        List<Monitor> LoadMonitorEntites(Expression<Func<Monitor, bool>> @where);

        int SaveChanges();
    }
}