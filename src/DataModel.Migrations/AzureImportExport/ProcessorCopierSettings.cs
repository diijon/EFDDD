using System;
using EFDDD.DataModel.Migrations.AzureImportExport.Azure;
using EFDDD.DataModel.Migrations.AzureImportExport.Contracts;

namespace EFDDD.DataModel.Migrations.AzureImportExport
{
    public class ProcessorCopierSettings : IProcessorCopierSettings
    {
        public string SourceConnectionString { get; set; }
        public string SourceAzureEndpointName { get; set; }
        public string DestinationConnectionString { get; set; }
        public string DestinationAzureEndpointName { get; set; }

        public ProcessorCopierSettings(string sourceConnectionString, string sourceAzureEndpointName, string destinationConnectionString, string destinationAzureEndpointName)
        {
            if (string.IsNullOrEmpty(sourceConnectionString)) { throw new ArgumentException("Cannot not equal null or empty", "sourceConnectionString"); }
            if (string.IsNullOrEmpty(sourceAzureEndpointName)) { throw new ArgumentException("Cannot not equal null or empty", "sourceAzureEndpointName"); }
            if (string.IsNullOrEmpty(destinationConnectionString)) { throw new ArgumentException("Cannot not equal null or empty", "destinationConnectionString"); }
            if (string.IsNullOrEmpty(destinationAzureEndpointName)) { throw new ArgumentException("Cannot not equal null or empty", "destinationAzureEndpointName"); }

            if (sourceAzureEndpointName.ParseEnum(Endpoint.None) == Endpoint.None) { throw new ArgumentException("Must be a recognized endpoint", "sourceAzureEndpointName"); }
            if (destinationAzureEndpointName.ParseEnum(Endpoint.None) == Endpoint.None) { throw new ArgumentException("Must be a recognized endpoint", "destinationAzureEndpointName"); }

            SourceConnectionString = sourceConnectionString;
            SourceAzureEndpointName = sourceAzureEndpointName;
            DestinationConnectionString = destinationConnectionString;
            DestinationAzureEndpointName = destinationAzureEndpointName;
        }
    }
}