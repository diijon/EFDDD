using System;
using System.Collections.Generic;
using System.Linq;
using EFDDD.DataModel.Migrations.Core.Contracts;

namespace EFDDD.DataModel.Migrations.Core
{
    public class MigrationRunnerSettings : IMigrationRunnerSettings
    {
        public IList<string> DatabasesToMigrate { get; private set; }
        public string MigrationName { get; private set; }
        public string ServerName { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string DbSchema { get; private set; }
        public bool DeleteDatabaseBackups { get; private set; }
        public bool SkipAzureCopies { get; private set; }
        public string AzureCopyDataCenter { get; private set; }
        public string AzureCopyStorageConnectionString { get; private set; }
        public string AzureCopyStorageContainer { get; private set; }
        public int RetryCount { get; private set; }

        public MigrationRunnerSettings(string serverName, string userName, string password, IList<string> databasesToMigrate, string migrationName, bool deleteDatabaseBackups, bool skipAzureCopies, string azureCopyDataCenter, string azureCopyStorageConnectionString, string azureCopyStorageContainer, string dbSchema, int retryCount)
        {
            if (string.IsNullOrEmpty(serverName)) { throw new ArgumentNullException("serverName"); }
            if (string.IsNullOrEmpty(userName)) { throw new ArgumentNullException("userName"); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentNullException("password"); }
            if (!(databasesToMigrate ?? new List<string>()).Any()) { throw new ArgumentOutOfRangeException("databasesToMigrate"); }

            DatabasesToMigrate = databasesToMigrate;
            ServerName = serverName;
            UserName = userName;
            Password = password;
            MigrationName = migrationName;
            DeleteDatabaseBackups = deleteDatabaseBackups;
            SkipAzureCopies = skipAzureCopies;
            AzureCopyDataCenter = azureCopyDataCenter;
            AzureCopyStorageConnectionString = azureCopyStorageConnectionString;
            AzureCopyStorageContainer = azureCopyStorageContainer;
            DbSchema = dbSchema;
            RetryCount = retryCount;
        }

        public string GetConnectionString(string databaseName)
        {
            return string.Format(
                "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};MultipleActiveResultSets=True",
                ServerName,
                databaseName,
                UserName,
                Password);
        }
    }
}