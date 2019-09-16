using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Command;
using FilesExplorerInDB_EF.EFModels;
using FilesExplorerInDB_EF.Interface;
using FilesExplorerInDB_Manager.Interface;

namespace FilesExplorerInDB_Manager.Implements
{
    public class MonitorManager : IMonitorManager
    {
        private readonly IMonitorService _dbService = UnityContainerHelp.GetServer<IMonitorService>();
        //private readonly IMonitorMongoDbService _dbService = UnityContainerHelp.GetServer<IMonitorMongoDbService>();
        private Monitor _monitor;

        #region 基础操作

        private Monitor MonitorAdd(Monitor entity, bool isSave)
        {
            entity = _dbService.MonitorAdd(entity);
            if (isSave)
                SaveChanges();
            return entity;
        }

        private Monitor MonitorFind(params object[] keyValue)
        {
            return _dbService.MonitorFind(keyValue);
        }

        private IQueryable<Monitor> LoadMonitorEntites(Expression<Func<Monitor, bool>> where)
        {
            return _dbService.LoadMonitorEntites(where);
        }

        private void MonitorModified(Monitor files, bool isSave = false)
        {
            _dbService.MonitorModified(files);
            if (isSave) SaveChanges();
        }

        private int SaveChanges()
        {
            return _dbService.SaveChanges();
        }

        #endregion

        #region 定义的操作类别、信息类别、用户类别

        public enum OpType
        {
            AddFile,
            AddFolder,
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

        private String[] Operation { get; } =
        {
            "添加文件", //AddFile,
            "新建文件夹", //AddFolder,
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
            return Operation[Convert.ToInt32(op)];
        }

        public enum MessageType
        {
            Primary,
            Success,
            Info,
            Warning,
            Danger
        }

        private String[] MessageTypeName { get; } =
        {
            "主要", //Primary,
            "成功", //Success,
            "提示", //Info,
            "警告", //Warning,
            "错误" //Danger
        };

        public string GetMessageTypeName(MessageType op)
        {
            return MessageTypeName[Convert.ToInt32(op)];
        }

        public enum Operator
        {
            User,
            System
        }

        private String[] OperatorName { get; } =
        {
            "用户", //User,
            "系统" //System
        };

        public string GetOperatorName(Operator op)
        {
            return OperatorName[Convert.ToInt32(op)];
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
        public void AddMonitorRecord(MessageType messageType, OpType opType, Operator operatorName, string objectName,
            string message)
        {
            _monitor = UnityContainerHelp.GetServer<Monitor>();
            _monitor.Message = message;
            _monitor.MessageType = GetMessageTypeName(messageType);
            _monitor.ObjectName = objectName;
            _monitor.OperationType = GetOperation(opType);
            _monitor.Operator = GetOperatorName(operatorName);
            _monitor.Time = DateTime.Now;
            MonitorAdd(_monitor, true);
        }

        /// <summary>
        /// 监控 - 错误
        /// </summary>
        /// <param name="exception">错误对象</param>
        public void ErrorRecord(Exception exception)
        {
            string message = "\n\r";
            if (exception != null)
            {
                message += "异常信息：\n\r" + exception.Message;
                message += "\n\r";
                message += "输出信息：错误位置";
                message += "位置信息：\n\r" + exception.StackTrace;
                message += "\n\r";

                if (exception.InnerException != null)
                {
                    message += "异常信息：\n\r" + exception.InnerException.Message;
                    message += "\n\r";
                    message += "输出信息：错误位置\n\r";
                    message += "位置信息：\n\r" + exception.InnerException.StackTrace;
                    message += "\n\r";
                }

                AddMonitorRecord(MessageType.Danger, OpType.SystemError, Operator.System, exception.Source, message);
            }
        }

        /// <summary>
        /// 监控 - 添加文件
        /// </summary>
        /// <param name="objectName">源文件完整名称+路径</param>
        /// <param name="files">目标文件</param>
        public void AddFileRecord(string objectName, Files files)
        {
            string message = "\n\r";
            message += "原文件：" + objectName + "；\n\r";
            message += "复制到：" + files.RealName + "；\n\r";
            message += "文件标识ID：" + files.FileId + "；\n\r";
            message += "父文件夹标识ID" + files.FolderLocalId + "；\n\r";
            AddMonitorRecord(MessageType.Primary, OpType.AddFile, Operator.User, objectName, message);
        }

        /// <summary>
        /// 监控 - 删除 - 文件
        /// </summary>
        /// <param name="files">目标文件</param>
        public void DeleteFileRecord(Files files)
        {
            string message = "\n\r";
            message += "目标文件：" + files.FileName + "；\n\r";
            message += "文件标识ID：" + files.FileId + "；\n\r";
            message += "父文件夹标识ID" + files.FolderLocalId + "；\n\r";
            AddMonitorRecord(MessageType.Warning, OpType.SetDeleteState, Operator.User, files.FileName, message);
        }

        /// <summary>
        /// 监控 - 删除 - 文件夹
        /// </summary>
        /// <param name="folders">目标文件夹</param>
        public void DeleteFolderRecord(Folders folders)
        {
            string message = "\n\r";
            message += "目标文件夹：" + folders.FolderName + "；\n\r";
            message += "文件夹标识ID：" + folders.FolderId + "；\n\r";
            message += "父文件夹标识ID" + folders.FolderLocalId + "；\n\r";
            AddMonitorRecord(MessageType.Warning, OpType.SetDeleteState, Operator.User, folders.FolderName, message);
        }

        /// <summary>
        /// 监控 - 完全删除文件
        /// </summary>
        /// <param name="files">目标文件</param>
        /// <param name="fileInfo">物理文件信息</param>
        public void DeleteCompleteRecord(Files files, FileInfo fileInfo)
        {
            string message = "\n\r";
            message += "目标文件：" + files.FileName + "；\n\r";
            message += "文件标识ID：" + files.FileId + "；\n\r";
            message += "父文件夹标识ID" + files.FolderLocalId + "；\n\r";
            message += "物理文件：" + fileInfo.FullName;
            AddMonitorRecord(MessageType.Warning, OpType.CompleteDeleted, Operator.User, files.FileName, message);
        }

        /// <summary>
        /// 监控 - 新建文件夹
        /// </summary>
        /// <param name="folders">新建文件夹</param>
        public void AddFolderRecord(Folders folders)
        {
            string message = "\n\r";
            message += "新建文件夹：" + folders.FolderName + "；\n\r";
            message += "文件夹标识ID：" + folders.FolderId + "；\n\r";
            message += "父文件夹标识ID" + folders.FolderLocalId + "；\n\r";
            AddMonitorRecord(MessageType.Primary, OpType.AddFolder, Operator.User, folders.FolderName, message);
        }
    }
}