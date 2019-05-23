namespace FilesExplorerInDB_EF.EFModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Interface;

    public partial class Files : IFiles
    {
        [Key]
        public int FileId { get; set; }

        [Required]
        public string FileName { get; set; }

        public string FileType { get; set; }

        public int FolderLocalId { get; set; }

        public long Size { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ModifyTime { get; set; }

        public DateTime AccessTime { get; set; }

        public bool IsDelete { get; set; }

        public string RealName { get; set; }

        public bool IsMiss { get; set; }

        public virtual Folders Folders { get; set; }
    }
}
