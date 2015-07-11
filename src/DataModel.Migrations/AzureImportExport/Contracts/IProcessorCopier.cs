using System.Threading.Tasks;

namespace EFDDD.DataModel.Migrations.AzureImportExport.Contracts
{
    public interface IProcessorCopier : IProcessor
    {
        Task<string> Copy();
    }
}