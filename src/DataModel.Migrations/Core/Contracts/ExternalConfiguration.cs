using System;

namespace EFDDD.DataModel.Migrations.Core.Contracts
{
    public class ExternalConfiguration : IExternalConfiguration
    {
        public virtual void Seed(string connectionString, string connectionClient)
        {
            throw new NotImplementedException();
        }
    }
}