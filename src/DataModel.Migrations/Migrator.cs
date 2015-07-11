using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using EFDDD.DataModel.Migrations.Core.Contracts;

namespace EFDDD.DataModel.Migrations
{
    public class Migrator : DbMigrator, IMigrator
    {
        public Migrator(IMigrationsConfigurationSettings configuration)
            : base(new Migrations.Configuration
            {
                TargetDatabase = new DbConnectionInfo(configuration.ConnectionString, configuration.ConnectionClient)
            }) { }

        public IMigrator GetNewInstance(string connectionString, string connectionClient)
        {
            return Build(connectionString, connectionClient);
        }

        public static IMigrator Build(string connectionString, string connectionClient)
        {
            return new Migrator(new MigrationsConfigurationSettings(connectionString, connectionClient ?? "System.Data.SqlClient"));
        }
    }
}