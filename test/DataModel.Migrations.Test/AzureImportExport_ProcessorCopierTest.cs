using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFDDD.DataModel.Migrations.AzureImportExport;
using EFDDD.DataModel.Migrations.AzureImportExport.Azure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;

namespace DataModel.Migrations.Test
{
    [TestClass]
    public class AzureImportExport_ProcessorCopierTest
    {
        private ILogger _log;
        private string _connectionStringTemplate;
        private string _datacenter;
        private Dictionary<string, string> _azureStorage;
        private string _azureContainer;

        [TestInitialize]
        public void Initialize()
        {
            _log = new LoggerConfiguration()
                       .WriteTo.ColoredConsole(LogEventLevel.Debug)
                       .Enrich.With(new ProcessIdEnricher())
                       .CreateLogger();
            _connectionStringTemplate = Constants.AZURE_SQL_CONNECTIONSTRING_TEMPLATE;
            _datacenter = "EastUS";
            _azureStorage = Constants.AZURE_STORAGE_CONNECTIONSTRING.ToParsedAzureStorageConnection();
            _azureContainer = "azureimportexporttest";
        }

        private string GetConnectionString(string databasename = "master")
        {
            return _connectionStringTemplate.Replace("{{DATABASE_NAME}}", databasename);
        }
        [TestClass]
        public class AssertConfigurationIsValidMethod : AzureImportExport_ProcessorCopierTest
        {
            [TestMethod]
            public async Task ShouldEqualTrue()
            {
                bool actual;
                string sourceDbName = null, destDbName = null;
                try
                {
                    //Arrange
                    sourceDbName = string.Format("AzureIETest_{0:yyyyMMddHHmmssff}_{1:N}", DateTime.Now, Guid.NewGuid());
                    destDbName = sourceDbName + "_Copy";
                    await InternalHelpers.CreateDatabase(GetConnectionString(), sourceDbName);

                    var sourceConnectionString = GetConnectionString(sourceDbName);
                    var destConnectionString = GetConnectionString(destDbName);
                    var processorSettings = new ProcessorCopierSettings(sourceConnectionString, _datacenter, destConnectionString, _datacenter);
                    var importExportSettings = new ImportExportSettings(_azureStorage["AccountKey"], _azureStorage["AccountName"], _azureContainer);
                    var processor = new ProcessorCopier(_log, processorSettings, importExportSettings);

                    //Act
                    actual = await processor.AssertConfigurationIsValid();
                }
                finally
                {
                    try
                    {
                        InternalHelpers.DropDatabase(GetConnectionString(), sourceDbName).Wait();
                    }
                    catch { }
                }

                //Assert
                Assert.IsTrue(actual);
            }
        }

        [TestClass]
        public class StartMethod : AzureImportExport_ProcessorCopierTest
        {
            [TestMethod]
            public async Task ShouldNotThrowException()
            {
                bool actual;
                string sourceDbName = null, destDbName = null;
                try
                {
                    //Arrange
                    sourceDbName = string.Format("AzureIETest_{0:yyyyMMddHHmmssff}_{1:N}", DateTime.Now, Guid.NewGuid());
                    destDbName = sourceDbName + "_Copy";
                    await InternalHelpers.CreateDatabase(GetConnectionString(), sourceDbName);

                    var sourceConnectionString = GetConnectionString(sourceDbName);
                    var destConnectionString = GetConnectionString(destDbName);
                    var processorSettings = new ProcessorCopierSettings(sourceConnectionString, _datacenter, destConnectionString, _datacenter);
                    var importExportSettings = new ImportExportSettings(_azureStorage["AccountKey"], _azureStorage["AccountName"], _azureContainer);
                    var processor = new ProcessorCopier(_log, processorSettings, importExportSettings);

                    //Act
                    await processor.Start(1, onRetryException: ex =>
                    {
                        throw ex;
                    });
                }
                finally
                {
                    try
                    {
                        InternalHelpers.DropDatabase(GetConnectionString(), sourceDbName).Wait();
                        InternalHelpers.DropDatabase(GetConnectionString(), destDbName).Wait();
                    }
                    catch { }
                }

                //Assert
            }
        }
    }
}
