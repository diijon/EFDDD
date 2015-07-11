using System.Threading.Tasks;

namespace EFDDD.DataModel.Migrations.AzureImportExport.Contracts
{
    public interface IProcessorExporter : IProcessor
    {
        Task<string> Export();
    }
}