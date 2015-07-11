using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDDD.DataModel.Migrations.Core.ConsoleRunner
{
    public interface ICommandLineOptions
    {
        /// <summary>
        /// Gets or sets the Database Server
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Gets or sets the Database Server Login
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Gets or sets the Database Server Password
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the list of Database Names to Migrate
        /// </summary>
        List<string> Databases { get; set; }

        /// <summary>
        /// Gets or sets if database backups created before migration should be deleted
        /// </summary>
        bool DeleteBackups { get; set; }

        /// <summary>
        /// Gets or sets if Azure databases should migrate safely by migrating against a copy of the database instead of the database itself
        /// </summary>
        bool SkipAzureCopy { get; set; }

        /// <summary>
        /// Gets or sets the name of the migration to target. If set to null, the most recent migration is selected
        /// </summary>
        string Migration { get; set; }

        /// <summary>
        /// Gets or sets the name of the Azure Data Center. <seealso cref="http://azure.microsoft.com/en-us/regions/#services"/>
        /// </summary>
        string AzCopyDataCenter { get; set; }

        /// <summary>
        /// Gets or sets the azure storage string where database copies are placed
        /// </summary>
        string AzCopyStorage { get; set; }

        /// <summary>
        /// Gets or sets the azure storage container where database copies are placed
        /// </summary>
        string AzCopyContainer { get; set; }

        /// <summary>
        /// Gets or sets the Database Table Schema
        /// </summary>
        string DbSchema { get; set; }

        int RetryCount { get; set; }
    }
}
