namespace FilesExplorerInDB_EF.EFModels
{
    using Interface;
    using MongoDB.Bson.Serialization.Attributes;
    using Sikiro.Nosql.Mongo.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Mongo("FilesExplorerDB", "Files")]
    [Table("Files")]
    public class Files : MongoEntity, IFiles
    {
        /// <summary>
        /// 解决Sikiro.Nosql.Mongo中内置的_Id与MSSQL中的字段不相符的问题
        /// </summary>
        [NotMapped]
        protected new string Id { get; set; }

        [Key]
        public int FileId { get; set; }

        [Required]
        public string FileName { get; set; }

        public string FileType { get; set; }

        public int FolderLocalId { get; set; }

        public long Size { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreationTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ModifyTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime AccessTime { get; set; }

        public bool IsDelete { get; set; }

        public string RealName { get; set; }

        public bool IsMiss { get; set; }

        public virtual Folders Folders { get; set; }
    }
}
