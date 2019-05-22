namespace FilesExplorerInDB_EF.EFModels
{
    using System.Data.Entity;
    using Interface;

    public partial class FilesDB : DbContext, IFilesDB
    {
        public FilesDB()
            : base("name=FilesDB")
        {
        }

        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<Folders> Folders { get; set; }
        public virtual DbSet<Monitor> Monitor { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Folders>()
                .HasMany(e => e.Files)
                .WithRequired(e => e.Folders)
                .HasForeignKey(e => e.FolderLocalId)
                .WillCascadeOnDelete(false);
        }
    }
}