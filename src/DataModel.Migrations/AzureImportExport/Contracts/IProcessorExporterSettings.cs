namespace EFDDD.DataModel.Migrations.AzureImportExport.Contracts
{
    public interface IProcessorExporterSettings
    {
        string SourceConnectionString { get; set; }
        string SourceAzureEndpointName { get; set; }
    }
}