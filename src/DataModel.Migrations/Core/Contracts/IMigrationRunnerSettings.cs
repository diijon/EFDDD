using System.Collections.Generic;

namespace EFDDD.DataModel.Migrations.Core.Contracts
{
    public interface IMigrationRunnerSettings
    {
        IList<string> DatabasesToMigrate { get; }
        string MigrationName { get; }
        string ServerName { get; }
        string UserName { get; }
        string Password { get; }
        bool DeleteDatabaseBackups { get; }
        bool SkipAzureCopies { get; }
        string AzureCopyDataCenter { get; }
        string AzureCopyStorageConnectionString { get; }
        string AzureCopyStorageContainer { get; }
        string DbSchema { get; }
        int RetryCount { get; }

        string GetConnectionString(string databaseName);
    }
}