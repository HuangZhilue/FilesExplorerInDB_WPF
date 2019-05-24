using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Manager.Implments;

namespace FilesExplorerInDB_Manager.Interface
{
    public interface IMonitorManager
    {
        string GetOperation(MonitorManager.OpType op);

        string GetMessageTypeName(MonitorManager.MessageType op);

        string GetOperatorName(MonitorManager.Operator op);

        void AddMonitorRecord(MonitorManager.MessageType messageType, MonitorManager.OpType opType,
            MonitorManager.Operator operatorName, string objectName, string message);

        void ErrorRecord(string objectName, string message);

        void AddFileRecord(string objectName, Files files);

        void DeleteFileRecord(Files files);

        void AddFolderRecord(Folders folders);

        void DeleteFolderRecord(Folders folders);
    }
}