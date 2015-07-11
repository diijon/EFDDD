using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EFDDD.DataModel.Migrations.AzureImportExport.Azure;
using EFDDD.DataModel.Migrations.AzureImportExport.Contracts;
using EFDDD.DataModel.Migrations.AzureImportExport.Internal;
using Serilog;

namespace EFDDD.DataModel.Migrations.AzureImportExport
{
    public class ProcessorExporter : IProcessorExporter
    {
        private readonly ILogger _log;
        private readonly IImportExportSettings _importExportSettings;
        private readonly IProcessorExporterSettings _processorSettings;

        public ProcessorExporter(ILogger log, IProcessorExporterSettings processorSettings, IImportExportSettings importExportSettings)
        {
            if (log == null) { throw new ArgumentNullException("log"); }
            if (processorSettings == null) { throw new ArgumentNullException("processorSettings"); }
            if (importExportSettings == null) { throw new ArgumentNullException("importExportSettings"); }

            _log = log;
            _processorSettings = processorSettings;
            _importExportSettings = importExportSettings;
        }

        public async Task<string> Start(int retryCount = 1, int retryDelayInMilliseconds = 2500, Action<Exception> onRetryException = null)
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

            Func<string> syncFunc = () => Export().Result;
            return Helpers.Retry(syncFunc, retryCount, retryDelayInMilliseconds, onRetryException);
        }

        public async Task<bool> AssertConfigurationIsValid()
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

            return true;
        }

        public async Task<string> Export()
        {
            var sourceConnBuilder = new SqlConnectionStringBuilder(_processorSettings.SourceConnectionString);

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