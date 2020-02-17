using Command;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using FilesExplorerInDB_Manager.Interface;
using FilesExplorerInDB_Models.Models;
using Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using static System.Activator;

namespace FilesExplorerInDB_Manager.Implements
{
    public class MonitorManager : IMonitorManager
    {
        private IMonitorService DBService { get; } = UnityContainerHelp.GetServer<IMonitorService>();
        private Monitor _monitor;

        #region 基础操作

        private Monitor MonitorAdd(Monitor entity, bool isSave, bool autoId = false)
        {
            entity = DBService.MonitorAdd(entity,autoId);
            if (isSave) SaveChanges();
            return entity;
        }

        private Monitor MonitorFind(params object[] keyValue)
        {
            return DBService.MonitorFind(keyValue);
        }

        private List<Monitor> LoadMonitorEntities(Expression<Func<Monitor, bool>> where)
        {
            return DBService.LoadMonitorEntities(where);
        }

        private void MonitorModified(Monitor files, bool isSave = false)
        {
            DBService.MonitorModified(files);
            if (isSave) SaveChanges();
        }

        private int SaveChanges()
        {
            return DBService.SaveChanges();
        }

        #endregion

        public List<LogProperty> GetMessageList()
        {
            var mList = LoadMonitorEntities(l => l.Time > DateTime.MinValue);
            var logList = mList.Select(l => new LogProperty
            {
                Message = l.Message,
                MessageType = l.MessageType,
                ObjectName = l.ObjectName,
                Operator = l.Operator,
                OperationType = l.OperationType,
                Time = l.Time
            }).OrderByDescending(l => l.Time).ToList();
            return logList;
        }

        #region 定义的操作类别、信息类别、用户类别

        public enum OpType
        {
            AddFile,
            AddFolder,
            SetRestoreState,
            SetDeleteState,
            CompleteDeleted,
            RenameFile,
            RenameFolder,
            CopyPath,
            CutPath,
            ScanSystem,
            SetMissState,
            SystemError
        }

        private string[] Operation { get; } =
        {
            "添加文件", //AddFile,
            "新建文件夹", //AddFolder,
            "还原", //SetDeleteState,
            "标记为删除", //SetDeleteState,
            "完全删除", //CompleteDeleted,
            "重命名文件", //RenameFile,
            "重命名文件夹", //RenameFolder,
            "复制", //CopyPath,
            "剪切", //CutPath,
            "扫描系统", //ScanSystem,
            "标记为丢失", //SetMissState,
            "系统错误" //SystemError
        };

        public string GetOperation(OpType op)
        {
            return Operation[Convert.ToInt32(op,CultureInfo.CurrentCulture)];
        }

        public enum MessageType
        {
            Primary,
            Success,
            Info,
            Warning,
            Danger
        }

        private string[] MessageTypeName { get; } =
        {
            "主要", //Primary,
            "成功", //Success,
            "提示", //Info,
            "警告", //Warning,
            "错误" //Danger
        };

        public string GetMessageTypeName(MessageType op)
        {
            return MessageTypeName[Convert.ToInt32(op, CultureInfo.CurrentCulture)];
        }

        public enum OperatorName
        {
            User,
            System
        }

        private string[] OperatorType { get; } =
        {
            "用户", //User,
            "系统" //System
        };

        public string GetOperatorType(OperatorName op)
        {
            return OperatorType[Convert.ToInt32(op, CultureInfo.CurrentCulture)];
        }

        #endregion

        /// <summary>
        /// 添加监控记录
        /// </summary>
        /// <param name="messageType">信息类型</param>
        /// <param name="opType">操作类型</param>
        /// <param name="operatorName">操作员</param>
        /// <param name="objectName">操作对象</param>
        /// <param name="message">消息</param>
        public void AddMonitorRecord(MessageType messageType, OpType opType, OperatorName operatorName, string objectName,
            string message)
        {
            _monitor = (Monitor) CreateInstance(typeof(Monitor));
            _monitor.Message = message;
            _monitor.MessageType = GetMessageTypeName(messageType);
            _monitor.ObjectName = objectName;
            _monitor.OperationType = GetOperation(opType);
            _monitor.Operator = GetOperatorType(operatorName);
            _monitor.Time = DateTime.Now;
            MonitorAdd(_monitor, true, true);
        }

        /// <summary>
        /// 监控 - 错误
        /// </summary>
        /// <param name="exception">错误对象</param>
        public void ErrorRecord(Exception exception)
        {
            if (exception == null) return;
            var message = Environment.NewLine;
            message += "异常信息：" + Environment.NewLine + exception.Message;
            message += Environment.NewLine;
            message += "输出信息：错误位置";
            message += "位置信息：" + Environment.NewLine + exception.StackTrace;
            message += Environment.NewLine;

            GetInnerException(exception);

            AddMonitorRecord(MessageType.Danger, OpType.SystemError, OperatorName.System, exception.Source, message);
        }

        private static string GetInnerException(Exception e)
        {
            var message = "";
            if (e.InnerException == null) return message;
            message += "内部异常信息：" + Environment.NewLine + e.InnerException.Message;
            message += Environment.NewLine;
            message += "输出信息：错误位置" + Environment.NewLine;
            message += "位置信息：" + Environment.NewLine + e.InnerException.StackTrace;
            message += Environment.NewLine;
            if (e.InnerException.InnerException != null) message += GetInnerException(e.InnerException);

            return message;
        }

        /// <summary>
        /// 监控 - 添加文件
        /// </summary>
        /// <param name="objectName">源文件完整名称+路径</param>
        /// <param name="files">目标文件</param>
        public void AddFileRecord(string objectName, Files files)
        {
            if (files == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var message = Environment.NewLine;
            message += "原文件：" + objectName + "；" + Environment.NewLine;
            message += "复制到：" + files.RealName + "；" + Environment.NewLine;
            message += "文件标识ID：" + files.FileId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + files.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Primary, OpType.AddFile, OperatorName.User, objectName, message);
        }

        /// <summary>
        /// 监控 - 还原 - 文件
        /// </summary>
        /// <param name="files">目标文件</param>
        public void RestoreFileRecord(Files files)
        {
            if (files == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var message = Environment.NewLine;
            message += "目标文件：" + files.FileName + "；" + Environment.NewLine;
            message += "文件标识ID：" + files.FileId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + files.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Warning, OpType.SetRestoreState, OperatorName.User, files.FileName, message);
        }

        /// <summary>
        /// 监控 - 删除 - 文件
        /// </summary>
        /// <param name="files">目标文件</param>
        public void DeleteFileRecord(Files files)
        {
            if (files == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var message = Environment.NewLine;
            message += "目标文件：" + files.FileName + "；" + Environment.NewLine;
            message += "文件标识ID：" + files.FileId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + files.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Warning, OpType.SetDeleteState, OperatorName.User, files.FileName, message);
        }

        /// <summary>
        /// 监控 - 还原 - 文件夹
        /// </summary>
        /// <param name="folders">目标文件夹</param>
        public void RestoreFolderRecord(Folders folders)
        {
            if (folders == null) throw new Exception(Resource.Message_ArgumentNullException_Folders);
            var message = Environment.NewLine;
            message += "目标文件夹：" + folders.FolderName + "；" + Environment.NewLine;
            message += "文件夹标识ID：" + folders.FolderId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + folders.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Warning, OpType.SetRestoreState, OperatorName.User, folders.FolderName, message);
        }

        /// <summary>
        /// 监控 - 删除 - 文件夹
        /// </summary>
        /// <param name="folders">目标文件夹</param>
        public void DeleteFolderRecord(Folders folders)
        {
            if (folders == null) throw new Exception(Resource.Message_ArgumentNullException_Folders);
            var message = Environment.NewLine;
            message += "目标文件夹：" + folders.FolderName + "；" + Environment.NewLine;
            message += "文件夹标识ID：" + folders.FolderId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + folders.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Warning, OpType.SetDeleteState, OperatorName.User, folders.FolderName, message);
        }

        /// <summary>
        /// 监控 - 完全删除文件
        /// </summary>
        /// <param name="files">目标文件</param>
        public void DeleteFileCompleteRecord(Files files)
        {
            if (files == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var message = Environment.NewLine;
            message += "目标文件：" + files.FileName + "；" + Environment.NewLine;
            message += "文件标识ID：" + files.FileId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + files.FolderLocalId + "；" + Environment.NewLine;
            message += "物理文件：" + files.RealName;
            AddMonitorRecord(MessageType.Warning, OpType.CompleteDeleted, OperatorName.User, files.FileName, message);
        }

        /// <summary>
        /// 监控 - 完全删除文件夹
        /// </summary>
        /// <param name="folders">目标文件夹</param>
        public void DeleteFolderCompleteRecord(Folders folders)
        {
            if (folders == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var message = Environment.NewLine;
            message += "目标文件夹：" + folders.FolderName + "；" + Environment.NewLine;
            message += "文件夹标识ID：" + folders.FolderId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + folders.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Warning, OpType.CompleteDeleted, OperatorName.User, folders.FolderName, message);
        }

        /// <summary>
        /// 监控 - 新建文件夹
        /// </summary>
        /// <param name="folders">新建文件夹</param>
        public void AddFolderRecord(Folders folders)
        {
            if (folders == null) throw new Exception(Resource.Message_ArgumentNullException_Folders);
            var message = Environment.NewLine;
            message += "新建文件夹：" + folders.FolderName + "；" + Environment.NewLine;
            message += "文件夹标识ID：" + folders.FolderId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + folders.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Primary, OpType.AddFolder, OperatorName.User, folders.FolderName, message);
        }

        /// <summary>
        /// 监控 - 重命名文件夹
        /// </summary>
        /// <param name="folders">重命名文件夹</param>
        /// <param name="oldName">旧文件夹名称</param>
        public void RenameFolderRecord(Folders folders, string oldName)
        {
            if (folders == null) throw new Exception(Resource.Message_ArgumentNullException_Folders);
            var message = Environment.NewLine;
            message += "重命名文件夹：" + oldName + " --> " + folders.FolderName + "；" + Environment.NewLine;
            message += "文件夹标识ID：" + folders.FolderId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + folders.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Primary, OpType.RenameFolder, OperatorName.User, folders.FolderName, message);
        }

        /// <summary>
        /// 监控 - 重命名文件
        /// </summary>
        /// <param name="files">重命名文件</param>
        /// <param name="oldName">旧文件名称</param>
        public void RenameFileRecord(Files files, string oldName)
        {
            if (files == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var message = Environment.NewLine;
            message += "重命名文件：" + oldName + " --> " + files.FileName + "；" + Environment.NewLine;
            message += "文件标识ID：" + files.FileId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + files.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Primary, OpType.RenameFile, OperatorName.User, files.FileName, message);
        }

        /// <summary>
        /// 监控 - 复制文件
        /// </summary>
        /// <param name="files">复制的文件</param>
        /// <param name="oldFolderLocalId">原父文件夹标识ID</param>
        public void CopyFileRecord(Files files, string oldFolderLocalId)
        {
            if (files == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var message = Environment.NewLine;
            message += "文件：" + files.FileName + "；" + Environment.NewLine;
            message += "文件标识ID：" + files.FileId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + oldFolderLocalId + " --> " + files.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Primary, OpType.CopyPath, OperatorName.User, files.FileName, message);
        }

        /// <summary>
        /// 监控 - 复制文件夹
        /// </summary>
        /// <param name="folders">复制的文件夹</param>
        /// <param name="newFolderLocalId">新父文件夹标识ID</param>
        public void CopyFolderRecord(Folders folders, string newFolderLocalId)
        {
            if (folders == null) throw new Exception(Resource.Message_ArgumentNullException_Folders);
            var message = Environment.NewLine;
            message += "文件夹：" + folders.FolderName + "；" + Environment.NewLine;
            message += "文件夹标识ID：" + folders.FolderId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + folders.FolderLocalId + " --> " + newFolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Primary, OpType.CopyPath, OperatorName.User, folders.FolderName, message);
        }

        /// <summary>
        /// 监控 - 剪切文件
        /// </summary>
        /// <param name="files">剪切的文件</param>
        /// <param name="oldFolderLocalId">原父文件夹标识ID</param>
        public void CutFileRecord(Files files, string oldFolderLocalId)
        {
            if (files == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var message = Environment.NewLine;
            message += "文件：" + files.FileName + "；" + Environment.NewLine;
            message += "文件标识ID：" + files.FileId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + oldFolderLocalId + " --> " + files.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Primary, OpType.CutPath, OperatorName.User, files.FileName, message);
        }

        /// <summary>
        /// 监控 - 剪切文件夹
        /// </summary>
        /// <param name="folders">剪切的文件夹</param>
        /// <param name="oldFolderLocalId">原父文件夹标识ID</param>
        public void CutFolderRecord(Folders folders, string oldFolderLocalId)
        {
            if (folders == null) throw new Exception(Resource.Message_ArgumentNullException_Folders);
            var message = Environment.NewLine;
            message += "文件夹：" + folders.FolderName + "；" + Environment.NewLine;
            message += "文件夹标识ID：" + folders.FolderId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + oldFolderLocalId + " --> " + folders.FolderLocalId + "；" + Environment.NewLine;
            AddMonitorRecord(MessageType.Primary, OpType.CutPath, OperatorName.User, folders.FolderName, message);
        }

        /// <summary>
        /// 监控 - 标记丢失的文件
        /// </summary>
        /// <param name="files">丢失的文件</param>
        public void SetMissStateRecord(Files files)
        {
            if (files == null) throw new Exception(Resource.Message_ArgumentNullException_Files);
            var message = Environment.NewLine;
            message += "文件：" + files.FileName + "；" + Environment.NewLine;
            message += "文件标识ID：" + files.FileId + "；" + Environment.NewLine;
            message += "父文件夹标识ID：" + files.FolderLocalId + "；" + Environment.NewLine;
            message += "物理文件：" + files.RealName;
            AddMonitorRecord(MessageType.Warning, OpType.SetMissState, OperatorName.User, files.FileName, message);
        }
    }
}