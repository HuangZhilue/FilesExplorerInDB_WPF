namespace FilesExplorerInDB_EF.EFModels
{
    using Interface;
    using MongoDB.Bson.Serialization.Attributes;
    using Sikiro.Nosql.Mongo.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Mongo("FilesExplorerDB", "Folders")]
    [Table("Folders")]
    public sealed class Folders : MongoEntity, IFolders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Folders()
        {
            Files = new HashSet<Files>();
        }

        /// <summary>
        /// 解决Sikiro.Nosql.Mongo中内置的_Id与MSSQL中的字段不相符的问题
        /// </summary>
        [NotMapped]
        private new string Id { get; set; }


        [Key]
        public int FolderId { get; set; }

        public string FolderName { get; set; }

        public int FolderLocalId { get; set; }

        public long Size { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreationTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ModifyTime { get; set; }

        public int FolderIncludeCount { get; set; }

        public int FileIncludeCount { get; set; }

        public bool IsDelete { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Files> Files { get; set; }

        [NotMapped]
        public List<Folders> FolderNodes { get; set; }
    }
}
