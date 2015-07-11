using EFDDD.DataModel.EF;

namespace EFDDD.DataModel.Migrations
{
    public class MigrationContext : Context
    {
        public MigrationContext() : base("DataContext") { }

        public MigrationContext(string connectionString) : base(connectionString) { }
    }
}
