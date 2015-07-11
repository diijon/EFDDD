using System.Threading.Tasks;

namespace EFDDD.DataModel.Migrations.Core.Contracts
{
    public interface IMigrationRunner
    {
        Task Run();
    }
}