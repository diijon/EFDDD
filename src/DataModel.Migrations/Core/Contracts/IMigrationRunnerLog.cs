namespace EFDDD.DataModel.Migrations.Core.Contracts
{
    public interface IMigrationRunnerLog
    {
        void LogError(string message);
        void LogInfo(string message);
        void LogWarning(string message);
    }
}