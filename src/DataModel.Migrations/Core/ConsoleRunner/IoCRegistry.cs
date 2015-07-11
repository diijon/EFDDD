using System;
using EFDDD.DataModel.Migrations.Core.Contracts;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using StructureMap.Configuration.DSL;

namespace EFDDD.DataModel.Migrations.Core.ConsoleRunner
{
    public class IoCRegistry : Registry
    {
        public IoCRegistry() : this(null, null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger">If equals null, it's set to console logger</param>
        public IoCRegistry(ICommandLineOptions options, ILogger logger)
        {
            if (options == null) { throw new ArgumentNullException("options"); }
            logger = logger ?? GetLogger();

            For<ICommandLineOptions>().Use<CommandLineOptions>();

            For<ILogger>().Singleton().Use(() => logger);

            For<IMigrationRunnerSettings>().Use(() => new MigrationRunnerSettings(options.Server, options.UserName, options.Password, options.Databases, options.Migration, options.DeleteBackups, options.SkipAzureCopy, options.AzCopyDataCenter, options.AzCopyStorage, options.AzCopyContainer, options.DbSchema, options.RetryCount));

            var connectionString = string.Format(
                "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};MultipleActiveResultSets=True",
                options.Server,
                "master",
                options.UserName,
                options.Password);
            For<IMigrationsConfigurationSettings>().Use(() => new MigrationsConfigurationSettings(connectionString, null));

            For<IMigrationRunner>().Use<MigrationRunner>();
        }

        public ILogger GetLogger()
        {
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.ColoredConsole(LogEventLevel.Debug)
                .Enrich.With(new ProcessIdEnricher());
            return loggerConfig.CreateLogger();
        }
    }
}