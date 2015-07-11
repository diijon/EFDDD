using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EFDDD.DataModel.Migrations.AzureImportExport.Azure;
using EFDDD.DataModel.Migrations.AzureImportExport.Contracts;
using EFDDD.DataModel.Migrations.AzureImportExport.Internal;
using Serilog;

namespace EFDDD.DataModel.Migrations.AzureImportExport
{
    public class ProcessorCopier : IProcessorCopier
    {
        private readonly ILogger _log;
        private readonly IProcessorCopierSettings _processorSettings;
        private readonly IImportExportSettings _importExportSettings;

        public ProcessorCopier(ILogger log, IProcessorCopierSettings processorSettings, IImportExportSettings importExportSettings)
        {
            if (log == null) { throw new ArgumentNullException("log"); }
            if (processorSettings == null) { throw new ArgumentNullException("processorSettings"); }
            if (importExportSettings == null) { throw new ArgumentNullException("importExportSettings"); }

            _log = log;
            _processorSettings = processorSettings;
            _importExportSettings = importExportSettings;
        }

        public virtual async Task<string> Start(int retryCount = 1, int retryDelayInMilliseconds = 2500, Action<Exception> onRetryException = null)
        {
            var isConfigValid = await AssertConfigurationIsValid();
            if (!isConfigValid)
            {
                Log.Error("The Configuration is not valid");
                return null;
            }

            try
            {
                var storageAccount = Helpers.GetBlobAccount(_importExportSettings.StorageAccountName, _importExportSettings.StorageKey);
                Helpers.CreateBlobContainerIfNotExists(storageAccount, _importExportSettings.StorageContainer);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "The Storage Account is not valid");
                return null;
            }

            Func<string> syncFunc = () => Copy().Result;
            return Helpers.Retry(syncFunc, retryCount, retryDelayInMilliseconds, onRetryException);
        }

        public virtual async Task<bool> AssertConfigurationIsValid()
        {
            var souceDbConnectTask = Task.Run(() =>
            {
                using (var conn = new SqlConnection(_processorSettings.SourceConnectionString))
                {
                    try
                    {
                        conn.Open();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        _log.Error(ex, "Could not connect to {SourceDatabase}", _processorSettings.SourceConnectionString.ToSecretConnectionString());
                        throw new Exception(string.Format("Could not connect to {0}", _processorSettings.SourceConnectionString.ToSecretConnectionString()), ex);
                    }
                }
            });
            var connect = await souceDbConnectTask;
            if (!connect) { return false; }

            var destDbExistsTask = Task.Run(() =>
            {
                var destConnBuilder = new SqlConnectionStringBuilder(_processorSettings.DestinationConnectionString);
                destConnBuilder.InitialCatalog = "master";
                using (var conn = new SqlConnection(destConnBuilder.ConnectionString))
                {
                    destConnBuilder = new SqlConnectionStringBuilder(_processorSettings.DestinationConnectionString);
                    try
                    {
                        conn.Open();
                        var exists = conn.Query<string>("SELECT name FROM sys.databases WHERE name = @databaseName", new { databaseName = destConnBuilder.InitialCatalog }).FirstOrDefault() != null;
                        if (exists) { _log.Error("Cannot restore to {DestinationDatabase} because the Database already exists", _processorSettings.DestinationConnectionString.ToSecretConnectionString()); }
                        return exists;
                    }
                    catch (SqlException ex)
                    {
                        _log.Error(ex, "Could not connect to {DestinationDatabase}", destConnBuilder.ConnectionString.ToSecretConnectionString());
                        throw new Exception(string.Format("Could not connect to {0}", destConnBuilder.ConnectionString.ToSecretConnectionString()), ex);
                    }
                }
            });
            var destDbExists = await destDbExistsTask;
            if (destDbExists)
            {
                _log.Error("Cannot restore Databases that exists");
                return false;
            }

            return true;
        }

        public virtual async Task<string> Copy()
        {
            var sourceConnBuilder = new SqlConnectionStringBuilder(_processorSettings.SourceConnectionString);
            var destConnBuilder = new SqlConnectionStringBuilder(_processorSettings.DestinationConnectionString);

            string exportBlobUri = null;
            try
            {
                var ieHelper = new ImportExportHelper(_log)
                {
                    EndPointUri = Helpers.GetEndpointUris()[_processorSettings.SourceAzureEndpointName.ParseEnum(Endpoint.None)],
                    StorageKey = _importExportSettings.StorageKey,
                    ServerName = sourceConnBuilder.DataSource.Replace("tcp:", string.Empty).Replace(",1433", string.Empty),
                    DatabaseName = sourceConnBuilder.InitialCatalog,
                    UserName = sourceConnBuilder.UserID,
                    Password = sourceConnBuilder.Password
                };

                exportBlobUri = string.Format("https://{0}.blob.core.windows.net/{1}/{2}-{3:yyyyMMddHHmmssff}.bacpac", _importExportSettings.StorageAccountName, _importExportSettings.StorageContainer, ieHelper.DatabaseName.ToLower(), DateTime.UtcNow);
                var exportBlobPath = ieHelper.DoExport(exportBlobUri);
                if (exportBlobPath == null)
                {
                    _log.Error("Could not complete the export step for Backup and Restore of {SourceDatabase} to {ExportBlobPath}", _processorSettings.SourceConnectionString.ToSecretConnectionString(), exportBlobUri);
                    throw new NullReferenceException("ImportExportHelper.DoExport()");
                }

                ieHelper.EndPointUri = Helpers.GetEndpointUris()[_processorSettings.DestinationAzureEndpointName.ParseEnum(Endpoint.None)];
                ieHelper.ServerName = destConnBuilder.DataSource.Replace("tcp:", string.Empty).Replace(",1433", string.Empty);
                ieHelper.DatabaseName = destConnBuilder.InitialCatalog;
                ieHelper.UserName = destConnBuilder.UserID;
                ieHelper.Password = destConnBuilder.Password;

                //var importSucceded = ieHelper.DoImport(exportBlobPath);
                var importSucceded = ieHelper.DoImport(exportBlobUri);
                if (importSucceded == false)
                {
                    _log.Error("Could not complete the import step for Backup and Restore of {DestinationDatabase} from {ExportBlobPath}", _processorSettings.DestinationConnectionString.ToSecretConnectionString(), exportBlobPath);
                    throw new NullReferenceException("ImportExportHelper.DoImport()");
                }

                return exportBlobPath;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Could not Backup {SourceDatabase} to {ExportBlobPath}", _processorSettings.SourceConnectionString.ToSecretConnectionString(), exportBlobUri ?? string.Empty);
                throw;
            }
        }
    }
}