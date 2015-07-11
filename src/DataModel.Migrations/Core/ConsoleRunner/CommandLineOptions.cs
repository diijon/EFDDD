using System.Collections.Generic;
using CommandLine;

namespace EFDDD.DataModel.Migrations.Core.ConsoleRunner
{
    public class CommandLineOptions : ICommandLineOptions
    {
        [Option('s', "server", DefaultValue = "./", Required = true, HelpText = "Gets or sets the Database Server")]
        public string Server { get; set; }

        [Option('u', "username", Required = true, HelpText = "Gets or sets the Database Server Login")]
        public string UserName { get; set; }

        [Option('p', "password", Required = true, HelpText = "Gets or sets the Database Server Password")]
        public string Password { get; set; }

        [OptionList('d', "databases", ',', Required = true, HelpText = "Gets or sets the list of Database Names to Migrate")]
        public List<string> Databases { get; set; }

        [Option('x', "deletebackups", DefaultValue = true, Required = false, HelpText = "Gets or sets if database backups created before migration should be deleted")]
        public bool DeleteBackups { get; set; }

        [Option('a', "skipazurecopy", DefaultValue = false, Required = false, HelpText = "Gets or sets if Azure databases should safely migrate by migrating against a copy of the database versus directly against the database")]
        public bool SkipAzureCopy { get; set; }

        [Option('m', "migration", Required = false, HelpText = "Gets or sets the name of the migration to target. If set to null, the most recent migration is selected")]
        public string Migration { get; set; }

        [Option('c', "azcopydatacenter", Required = true, HelpText = "Gets or sets the name of the Azure Data Center. See http://azure.microsoft.com/en-us/regions/#services")]
        public string AzCopyDataCenter { get; set; }

        [Option('o', "azcopystorage", Required = true, HelpText = "Gets or sets the azure storage string where database copies are placed")]
        public string AzCopyStorage { get; set; }

        [Option('n', "azcopycontainer", Required = true, HelpText = "Gets or sets the azure storage container where database copies are placed")]
        public string AzCopyContainer { get; set; }

        [Option('h', "dbschema", Required = false, HelpText = "Gets or sets the Database Table Schema", DefaultValue = "dbo")]
        public string DbSchema { get; set; }

        [Option('r', "retryCount", DefaultValue = 3, Required = false)]
        public int RetryCount { get; set; }
    }
}