namespace EFDDD.DataModel.Migrations.Core.Contracts
{
    public interface IMigrationsConfigurationSettings
    {
        string ConnectionString { get; set; }
        string ConnectionClient { get; set; }
    }
}