namespace EFDDD.DataModel.Migrations.Core.Contracts
{
    public class MigrationsConfigurationSettings : IMigrationsConfigurationSettings
    {
        public string ConnectionString { get; set; }
        public string ConnectionClient { get; set; }

        public MigrationsConfigurationSettings(string connectionString, string connectionClient)
        {
            ConnectionString = connectionString;
            ConnectionClient = connectionClient ?? "System.Data.SqlClient";
        }
    }
}