namespace EFDDD.DataModel.Migrations.AzureImportExport.Azure
{
    public interface IImportExportSettings
    {
        string StorageKey { get; set; }
        string StorageAccountName { get; set; }
        string StorageContainer { get; set; }
    }
}