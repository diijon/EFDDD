using System;
using EFDDD.DataModel.Migrations;
using EFDDD.DataModel.Migrations.Core.ConsoleRunner;
using EFDDD.DataModel.Migrations.Core.Contracts;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using ExternalConfiguration = EFDDD.DataModel.Migrations.Core.Contracts.ExternalConfiguration;
using StructureMap;

namespace EFDDD.Exe
{
    public class BootStrap
    {
        public static IContainer Strap(ICommandLineOptions options)
        {
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.ColoredConsole(LogEventLevel.Debug)
                .Enrich.With(new ProcessIdEnricher());
#if DEBUG
            /**/
            loggerConfig.WriteTo.File(string.Format(@"C:\Logs\DataModel.Migrations.{0:yyyyMMddHHmmssff}.txt", DateTime.Now));
            /**/
#endif

            var container = new Container(x =>
            {
                x.AddRegistry(new IoCRegistry(options, loggerConfig.CreateLogger()));
                x.For<IExternalConfiguration>().Use<ExternalConfiguration>();
                x.For<IMigrator>().Use(Migrator.Build("Data Source=(LocalDb)\v11.0;Initial Catalog=ThisConnectionIsNeverUsed;Integrated Security=SSPI;", null));
            });

            container.AssertConfigurationIsValid();
            return container;
        }
    }
}