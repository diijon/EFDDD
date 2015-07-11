using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDDD.DataModel.Migrations.AzureImportExport.Contracts
{
    public interface IProcessor
    {
        Task<string> Start(int retryCount = 1, int retryDelayInMilliseconds = 2500, Action<Exception> onRetryException = null);
        Task<bool> AssertConfigurationIsValid();
    }
}
