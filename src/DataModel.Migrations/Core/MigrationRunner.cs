using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EFDDD.DataModel.Migrations.AzureImportExport;
using EFDDD.DataModel.Migrations.AzureImportExport.Azure;
using EFDDD.DataModel.Migrations.Core.Contracts;
using Serilog;

namespace EFDDD.DataModel.Migrations.Core
{
    public class MigrationRunner : IMigrationRunner
    {
        #region Fields
        private const string DefaultConnectionClient = "System.Data.SqlClient";
        private readonly IExternalConfiguration ExternalConfiguration;
        private readonly IMigrationRunnerSettings Settings;
        private readonly ILogger Log;
        private readonly string MasterDbConnectionString;
        private readonly IMigrator Migrator;
        private static AzureCopierFactory _azureCopier;
        #endregion

        public MigrationRunner(ILogger log, IMigrator migrator, IMigrationRunnerSettings settings, IExternalConfiguration externalConfiguration)
        {
            ExternalConfiguration = externalConfiguration;
            Settings = settings;
            Log = log;
            Migrator = migrator;

            MasterDbConnectionString = settings.GetConnectionString("master");

            var azCopyStorageParsed = Settings.AzureCopyStorageConnectionString.ToParsedAzureStorageConnection();
            _azureCopier = new AzureCopierFactory
            {
                Logger = Log,
                ImportExportSettings = new ImportExportSettings(azCopyStorageParsed["AccountKey"], azCopyStorageParsed["AccountName"], Settings.AzureCopyStorageContainer)
            };
        }

        public async Task Run()
        {
            var canConnect = CanConnectToServer();
            if (!canConnect) { return; }

            var hasNecessaryPriveledges = HasCorrectPriviledges();
            if (!hasNecessaryPriveledges) { return; }

            var migrateDatabaseTasks = from databaseName in Settings.DatabasesToMigrate
                select Task.Run(() =>
                {
                    bool isNecessary;
                    var createdTargetDatabase = CreateTargetDatabase(out isNecessary, databaseName);
                    if (isNecessary && !createdTargetDatabase) { return; }

                    var migrations = GetModelMigrations(databaseName).ToList();
                    if (!migrations.Any()) { return; }

                    var migrationName = string.IsNullOrEmpty(Settings.MigrationName) ? migrations.Last() : Settings.MigrationName;
                    var hasMatchingMigration = HasMatchingMigration(migrations, migrationName);
                    if (!hasMatchingMigration) { return; }

                    string backupDb = null;
                    var isServerAzure = false;
                    if (!isNecessary)
                    {
                        var createdDbMigrationCopy = CreateDatabaseBackup(out backupDb, out isServerAzure, databaseName);
                        if (!createdDbMigrationCopy) { return; }
                    }

                    var migratedDb = MigrateDatabase(databaseName, migrationName, backupDb, isServerAzure);
                    if (!migratedDb) { return; }

                    var seededDb = SeedDatabase(databaseName, backupDb, isServerAzure);
                    if (!seededDb) { return; }

                    if (backupDb != null && Settings.DeleteDatabaseBackups)
                    {
                        DeleteDatabaseBackup(backupDb);
                    }

                    GetCompletedMigrations(databaseName);
                });

            await Task.WhenAll(migrateDatabaseTasks);
        }

        public IEnumerable<string> GetCompletedMigrations(string databaseName)
        {
            try
            {
                Log.Information("Retrieving completed migrations for Database {sourceDb}...", databaseName);
                var connectionString = Settings.GetConnectionString(databaseName);
                var migrator = Migrator.GetNewInstance(connectionString, DefaultConnectionClient);
                var allMigrations = migrator.GetDatabaseMigrations().ToList();

                Log.Information("The Migration was successful. Here are all Migrations that exist in Database {sourceDb} selected database: {@availableMigrations}", databaseName, allMigrations);
                return allMigrations;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "The completed migrations could not be identified");
            }

            return new List<string>();
        }

        public bool SeedDatabase(string databaseName, string backupDb, bool isServerAzure)
        {
            try
            {
                Log.Information("Seeding Data into Database {sourceDb}...", databaseName);
                var connectionString = Settings.GetConnectionString(databaseName);
                ExternalConfiguration.Seed(connectionString, DefaultConnectionClient);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while Seeding the Database");
                if (isServerAzure)
                {
                    RestoreBackupDatabase(backupDb, databaseName).Wait();
                }
            }

            return false;
        }

        public bool DeleteDatabaseBackup(string databaseName)
        {
            try
            {
                Log.Information("Deleting the selected database backup...");
                MigrationRunnerHelpers.DeleteDatabase(MasterDbConnectionString, databaseName);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting the selected database backup");
            }
            return false;
        }

        public bool MigrateDatabase(string databaseName, string migrationName, string backupDb, bool isServerAzure)
        {
            try
            {
                var hasMigration = MigrationRunnerHelpers.ContainsMigration(Settings.GetConnectionString(databaseName), Settings.DbSchema, migrationName);
                if (hasMigration)
                {
                    Log.Information("Database {sourceDb} contains Migration {targetMigration} and will skip the Migration step", databaseName, migrationName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while determining if Database {soureDb} has Migration {targetMigration}", databaseName, migrationName);
            }

            try
            {
                Log.Information("Migrating Database {sourceDb} to Migration {targetMigration}...", databaseName, migrationName);
                var connectionString = Settings.GetConnectionString(databaseName);
                var migrator = Migrator.GetNewInstance(connectionString, DefaultConnectionClient);
                migrator.Update(migrationName);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while attempting to migrate the Database {soureDb}", databaseName);
                if (isServerAzure)
                {
                    RestoreBackupDatabase(backupDb, databaseName).Wait();
                }
            }
            return false;
        }

        public async Task RestoreBackupDatabase(string backupDb, string originalDb)
        {
            try
            {
                var corruptDatabaseName = string.Format("{0}_DbMigratorCorrupt{1}", originalDb, DateTime.UtcNow.Ticks);
                Log.Information("Renaming {sourceDb} to {sourceDbCorruptName}...", originalDb, corruptDatabaseName);
                MigrationRunnerHelpers.RenameDatabase(originalDb, corruptDatabaseName, MasterDbConnectionString);

                Log.Information("Renaming {backupDb} to {sourceDb}...", backupDb, originalDb);
                MigrationRunnerHelpers.RenameDatabase(backupDb, originalDb, MasterDbConnectionString);
            }
            catch (Exception) { }
        }

        public bool CreateDatabaseBackup(out string backupDb, out bool isServerAzure, string databaseName)
        {
            backupDb = null;
            isServerAzure = MigrationRunnerHelpers.IsServerAzure(MasterDbConnectionString);
            if (!isServerAzure) { return true; }

            if (Settings.SkipAzureCopies) { return true; }

            backupDb = string.Format("{0}_DbMigratorBackup{1}", databaseName, DateTime.UtcNow.Ticks);
            try
            {
                Log.Information("Creating Database {backupDb} from copy of {sourceDb}", backupDb, databaseName);

                _azureCopier.CopierSettings = new ProcessorCopierSettings(Settings.GetConnectionString(databaseName), Settings.AzureCopyDataCenter, Settings.GetConnectionString(backupDb), Settings.AzureCopyDataCenter);
                var copierProcessor = _azureCopier.Build().Result;
                var randomDelay = RandomGen.Next(2000, 4000);
                copierProcessor.Start(Settings.RetryCount, randomDelay, ex =>
                {
                    throw ex;
                }).Wait();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Database {backupDb} could not be copied from {sourceDb}", backupDb, databaseName);
            }

            return false;
        }

        public bool HasMatchingMigration(IEnumerable<string> migrations, string migrationName)
        {
            try
            {
                var matchingMigration = migrations.FirstOrDefault(x => x.ToLower() == migrationName.ToLower());
                if (matchingMigration == null) { throw new ArgumentOutOfRangeException(); }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "The target Migration does not exist");
            }
            return false;
        }

        public IEnumerable<string> GetModelMigrations(string databaseName)
        {
            try
            {
                var connectionString = Settings.GetConnectionString(databaseName);
                var migrator = Migrator.GetNewInstance(connectionString, DefaultConnectionClient);
                return migrator.GetLocalMigrations();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "The model migrations could not be identified");
            }

            return new List<string>();
        }

        public bool CreateTargetDatabase(out bool isNecessary, string databaseName)
        {
            isNecessary = false;

            try
            {
                var doesDatabaseExist = MigrationRunnerHelpers.DoesDatabaseExist(MasterDbConnectionString, databaseName);
                if (doesDatabaseExist)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error encountered while determining if Database {sourceDb} exists", databaseName);
                return false;
            }

            try
            {
                isNecessary = true;
                Log.Information("Creating new Database {sourceDb}...", databaseName);
                MigrationRunnerHelpers.CreateDatabase(MasterDbConnectionString, databaseName);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "The new Database {sourceDb} could not be created", databaseName);
            }

            return false;
        }

        public bool HasCorrectPriviledges()
        {
            Log.Information("Testing Database Account priviledges...");
            var databaseName = string.Format("DbMigratorTest{0}", DateTime.UtcNow.Ticks);

            var hasCorrectCreationPriviledges = HasCorrectCreationPriviledges(databaseName);
            if (!hasCorrectCreationPriviledges) { return false; }

            var hasCorrectDropPriviledges = HasCorrectDropPriviledges(databaseName);
            if (!hasCorrectDropPriviledges) { return false; }

            Log.Information("The Database Account has the necessary priviledges to continue...");
            return true;
        }

        public bool HasCorrectDropPriviledges(string databaseName)
        {
            using (var conn = new SqlConnection(MasterDbConnectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        try
                        {
                            MigrationRunnerHelpers.SqlCmd(cmd, string.Format("DROP DATABASE [{0}]", databaseName));
                            Log.Information("Target Database Account can successfully Drop Databases");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Target Database Account cannot Drop Databases");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Target Database Account cannot Connect to the Server");
                }
            }
            return false;
        }

        public bool HasCorrectCreationPriviledges(string databaseName)
        {
            try
            {
                MigrationRunnerHelpers.CreateDatabase(MasterDbConnectionString, databaseName);
                Log.Information("Target Database Account can successfully Create Databases");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Target Database Account cannot Create Databases");
                return false;
            }
        }

        public bool CanConnectToServer()
        {
            Log.Information("Testing connection to Target Server...");

            using (var conn = new SqlConnection(MasterDbConnectionString))
            {
                try
                {
                    conn.Open();
                    Log.Information("Connection to Target Server was successful");
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Connection to Target Server was unsuccessful");
                }
            }

            return false;
        }
    }
}