using System.Linq;
using System.Text;

namespace EFDDD.DataModel.Migrations.Core.Contracts
{
    public interface IExternalConfiguration
    {
        void Seed(string connectionString, string connectionClient);
    }
}
