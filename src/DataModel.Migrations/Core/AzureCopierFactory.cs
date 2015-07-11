using System.Threading.Tasks;
using EFDDD.DataModel.Migrations.AzureImportExport;
using EFDDD.DataModel.Migrations.AzureImportExport.Azure;
using EFDDD.DataModel.Migrations.AzureImportExport.Contracts;
using Serilog;

namespace EFDDD.DataModel.Migrations.Core
{
    public class AzureCopierFactory
    {
        public ILogger Logger { get; set; }
        public IProcessorCopierSettings CopierSettings { get; set; }
        public IImportExportSettings ImportExportSettings { get; set; }

        public async Task<IProcessorCopier> Build()
        {
            return new ProcessorCopier(Logger, CopierSettings, ImportExportSettings);
        }
    }
}
