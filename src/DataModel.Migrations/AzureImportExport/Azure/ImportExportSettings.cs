using System;

namespace EFDDD.DataModel.Migrations.AzureImportExport.Azure
{
    public class ImportExportSettings : IImportExportSettings
    {
        public string StorageKey { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageContainer { get; set; }

        public ImportExportSettings(string storageKey, string storageAccountName, string storageContainer)
        {
            if (string.IsNullOrEmpty(storageKey)) { throw new ArgumentNullException("storageKey"); }
            if (string.IsNullOrEmpty(storageAccountName)) { throw new ArgumentNullException("storageAccountName"); }
            if (string.IsNullOrEmpty(storageContainer)) { throw new ArgumentNullException("storageContainer"); }

            StorageKey = storageKey;
            StorageAccountName = storageAccountName;
            StorageContainer = storageContainer;
        }
    }
}