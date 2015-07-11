using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Reflection;
using EFDDD.DataModel.Migrations.Core.Contracts;

namespace EFDDD.DataModel.Migrations
{
    public class ExternalConfiguration : IExternalConfiguration
    {
        private readonly DbMigrationsConfiguration _configuration;

        public ExternalConfiguration()
        {
            _configuration = new Migrations.Configuration();
        }

        public void Seed(string connectionString, string connectionClient)
        {
            _configuration.TargetDatabase = new DbConnectionInfo(connectionString, connectionClient);

            var type = _configuration.GetType();
            if (type == null) throw new InvalidOperationException();

            var methSeed = type.GetMethod("Seed", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (methSeed == null) throw new NullReferenceException("methSeed");

            var context = new MigrationContext(connectionString);
            methSeed.Invoke(_configuration, new object[] { context });
        }
    }
}