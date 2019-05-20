namespace FilesExplorerInDB_EF.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Interface;

    public partial class Folders : IFolders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Folders()
        {
            Files = new HashSet<Files>();
        }

        [Key]
        public int FolderId { get; set; }

        public string FolderName { get; set; }

        public int FolderLocalId { get; set; }

        public long Size { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ModifyTime { get; set; }

        public int FolderIncludeCount { get; set; }

        public int FileIncludeCount { get; set; }

        public bool IsDelete { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Files> Files { get; set; }

        [NotMapped]
        public List<Folders> FolderNodes { get; set; }
    }
}
