using System;
using EFDDD.DataModel.Migrations.AzureImportExport.Azure;
using EFDDD.DataModel.Migrations.AzureImportExport.Contracts;

namespace EFDDD.DataModel.Migrations.AzureImportExport
{
    public class ProcessorExporterSettings : IProcessorExporterSettings
    {
        public string SourceConnectionString { get; set; }
        public string SourceAzureEndpointName { get; set; }
        public ProcessorExporterSettings(string sourceConnectionString, string sourceAzureEndpointName)
        {
            if (string.IsNullOrEmpty(sourceConnectionString)) { throw new ArgumentException("Cannot not equal null or empty", "sourceConnectionString"); }
            if (string.IsNullOrEmpty(sourceAzureEndpointName)) { throw new ArgumentException("Cannot not equal null or empty", "sourceAzureEndpointName"); }

            if (sourceAzureEndpointName.ParseEnum(Endpoint.None) == Endpoint.None) { throw new ArgumentException("Must be a recognized endpoint", "sourceAzureEndpointName"); }

            SourceConnectionString = sourceConnectionString;
            SourceAzureEndpointName = sourceAzureEndpointName;
        }
    }
}
