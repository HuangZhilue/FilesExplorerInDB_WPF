namespace FilesExplorerInDB_EF.EFModels
{
    using System.Data.Entity;

    public sealed class FilesDB : DbContext//, IFilesDB
    {
        public static FilesDB GetFilesDb { get; } = new FilesDB();

        private FilesDB()
            : base("name=FilesDB")
        {
        }

        public DbSet<Files> Files { get; set; }
        public DbSet<Folders> Folders { get; set; }
        public DbSet<Monitor> Monitor { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Folders>()
                .HasMany(e => e.Files)
                .WithRequired(e => e.Folders)
                .HasForeignKey(e => e.FolderLocalId)
                .WillCascadeOnDelete(false);

            //modelBuilder.HasDefaultSchema("Oracle_User");
        }
    }
}