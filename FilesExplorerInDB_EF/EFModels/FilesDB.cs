using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.EntityFramework;
using Resources;
using static Resources.Properties.Settings;

namespace FilesExplorerInDB_EF.EFModels
{
    using System.Data.Entity;

    [DbConfigurationType(typeof(MultipleDbConfiguration))]
    public sealed class FilesDB : DbContext //, IFilesDB
    {
        public static FilesDB GetFilesDb { get; } = new FilesDB();

        public FilesDB()
            : base(MultipleDbConfiguration.GetMyConnection(), true)
        //: base("name=FilesDB")
        {
        }

        public DbSet<Files> Files { get; set; }
        public DbSet<Folders> Folders { get; set; }
        public DbSet<Monitor> Monitor { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new Exception(Resource.Message_ArgumentNullException_DbModelBuilder);
            modelBuilder.Entity<Folders>()
                .HasMany(e => e.Files)
                .WithRequired(e => e.Folders)
                .HasForeignKey(e => e.FolderLocalId)
                .WillCascadeOnDelete(false);

            //modelBuilder.HasDefaultSchema("Oracle_User");
        }
    }

    public class MultipleDbConfiguration : DbConfiguration
    {
        #region Constructors 

        public MultipleDbConfiguration()
        {
            //SetProviderServices(MySqlProviderInvariantName.ProviderName, new MySqlProviderServices());

            var dbType = GetSetting(SettingType.DBType).ToString();
            switch (dbType)
            {
                case "MySQL":
                    SetProviderServices(MySqlProviderInvariantName.ProviderName, new MySqlProviderServices());
                    break;
                case "SQL Server":
                    //TODO 似乎SQL Server并不需要在这里进行处理
                    //SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
                    break;
                case "Oracle":
                    SetProviderServices("Oracle.ManagedDataAccess.Client", EFOracleProviderServices.Instance);
                    break;
                //case "MongoDB":
                //    var mongodbFactory = new MySqlConnectionFactory();
                //    return mongodbFactory.CreateConnection(GetConnectionString());
                default:
                    throw new Exception(Resource.Message_ArgumentOutOfRangeException_DBType);
            }
        }

        #endregion Constructors

        #region Public methods 

        public static DbConnection GetMyConnection()
        {
            //var multipleDbConfiguration = new MultipleDbConfiguration();
            var dbType = GetSetting(SettingType.DBType).ToString();
            switch (dbType)
            {
                case "MySQL":
                    var mysqlFactory = new MySqlConnectionFactory();
                    return mysqlFactory.CreateConnection(GetConnectionString());
                case "SQL Server":
                    var mssqlFactory = new LocalDbConnectionFactory();
                    return mssqlFactory.CreateConnection(GetConnectionString());
                case "Oracle":
                    var oracleFactory = new OracleConnectionFactory();
                    return oracleFactory.CreateConnection(GetConnectionString());
                default:
                    throw new Exception(Resource.Message_ArgumentOutOfRangeException_DBType);
            }
        }

        #endregion Public methods
    }
}