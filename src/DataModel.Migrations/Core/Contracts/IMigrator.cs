using System.Collections.Generic;

namespace EFDDD.DataModel.Migrations.Core.Contracts
{
    public interface IMigrator
    {
        IEnumerable<string> GetDatabaseMigrations();
        IEnumerable<string> GetLocalMigrations();
        void Update(string targetMigration);
        IMigrator GetNewInstance(string connectionString, string connectionClient);
    }
}