using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Manager.Implements;
using System;
using System.IO;

namespace FilesExplorerInDB_Manager.Interface
{
    public interface IMonitorManager
    {
        string GetOperation(MonitorManager.OpType op);

        string GetMessageTypeName(MonitorManager.MessageType op);

        string GetOperatorType(MonitorManager.OperatorName op);

        void AddMonitorRecord(MonitorManager.MessageType messageType, MonitorManager.OpType opType,
            MonitorManager.OperatorName operatorName, string objectName, string message);

        void ErrorRecord(Exception exception);

        void AddFileRecord(string objectName, Files files);

        void DeleteFileRecord(Files files);

        void AddFolderRecord(Folders folders);

        void DeleteFolderRecord(Folders folders);

        void DeleteCompleteRecord(Files files, FileInfo fileInfo);

        void RenameFolderRecord(Folders folders, string oldName);

        void RenameFileRecord(Files files, string oldName);

        void CopyFileRecord(Files files, string newFolderLocalId);

        void CopyFolderRecord(Folders folders, string newFolderLocalId);

        void CutFileRecord(Files files, string oldFolderLocalId);

        void CutFolderRecord(Folders folders, string oldFolderLocalId);

        void SetMissStateRecord(Files files);
    }
}