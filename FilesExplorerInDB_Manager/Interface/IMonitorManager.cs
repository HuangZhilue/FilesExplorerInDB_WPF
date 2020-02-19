using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_Manager.Implements;
using System;
using System.Collections.Generic;
using FilesExplorerInDB_Models.Models;

namespace FilesExplorerInDB_Manager.Interface
{
    public interface IMonitorManager
    {
        List<LogProperty> GetMessageList(DateTime startTime, DateTime endTime, string message, string messageType,
            string objectName, string operatorName, string operationType);

        string GetOperation(MonitorManager.OpType op);

        string GetMessageTypeName(MonitorManager.MessageType op);

        string GetMessageTypeName(int messageTypeIndex);

        string GetOperatorType(MonitorManager.OperatorName op);

        string GetOperatorType(int operatorNameIndex);

        void AddMonitorRecord(MonitorManager.MessageType messageType, MonitorManager.OpType opType,
            MonitorManager.OperatorName operatorName, string objectName, string message);

        void ErrorRecord(Exception exception);

        void AddFileRecord(string objectName, Files files);

        void DeleteFileRecord(Files files);

        void AddFolderRecord(Folders folders);

        void RestoreFileRecord(Files files);

        void DeleteFolderRecord(Folders folders);

        void RestoreFolderRecord(Folders folders);

        void DeleteFileCompleteRecord(Files files);

        void DeleteFolderCompleteRecord(Folders folders);

        void RenameFolderRecord(Folders folders, string oldName);

        void RenameFileRecord(Files files, string oldName);

        void CopyFileRecord(Files files, string newFolderLocalId);

        void CopyFolderRecord(Folders folders, string newFolderLocalId);

        void CutFileRecord(Files files, string oldFolderLocalId);

        void CutFolderRecord(Folders folders, string oldFolderLocalId);

        void SetMissStateRecord(Files files);
    }
}