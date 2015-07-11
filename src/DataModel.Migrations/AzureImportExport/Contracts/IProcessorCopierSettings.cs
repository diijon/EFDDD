namespace EFDDD.DataModel.Migrations.AzureImportExport.Contracts
{
    public interface IProcessorCopierSettings
    {
        string SourceConnectionString { get; set; }
        string SourceAzureEndpointName { get; set; }
        string DestinationConnectionString { get; set; }
        string DestinationAzureEndpointName { get; set; }
    }
}