namespace FilesExplorerInDB_EF.EFModels
{
    using MongoDB.Bson.Serialization.Attributes;
    using Sikiro.Nosql.Mongo.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Mongo("FilesExplorerDB", "Monitor")]
    [Table("Monitor")]
    public class Monitor : MongoEntity
    {
        /// <summary>
        /// 解决Sikiro.Nosql.Mongo中内置的_Id与MSSQL中的字段不相符的问题
        /// </summary>
        [NotMapped]
        protected new string Id { get; set; }

        public int MonitorId { get; set; }

        [Required]
        public string MessageType { get; set; }

        [Required]
        public string OperationType { get; set; }

        public string ObjectName { get; set; }

        [Required]
        public string Operator { get; set; }

        public string Message { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }
    }
}
