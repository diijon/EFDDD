using System.Collections.Generic;
using EFDDD.DataModel.Migrations.WASDImportExport.SqlAzure;

namespace EFDDD.DataModel.Migrations.AzureImportExport.Azure
{
    public interface IImportExportHelper
    {
        string EndPointUri { get; set; }
        string StorageKey { get; set; }
        string ServerName { get; set; }
        string DatabaseName { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string DoExport(string blobUri);
        bool DoImport(string blobUri);
        List<StatusInfo> CheckRequestStatus(string requestGuid);
    }
}