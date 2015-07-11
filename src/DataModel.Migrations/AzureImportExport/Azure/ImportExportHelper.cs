using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;
using EFDDD.DataModel.Migrations.WASDImportExport.SqlAzure;
using Serilog;

namespace EFDDD.DataModel.Migrations.AzureImportExport.Azure
{
    internal class ImportExportHelper : IImportExportHelper
    {
        #region Fields and Properties
        private readonly ILogger _log;
        private string _endpointUri = "";
        private string _storageKey = "";
        private string _serverName = "";
        private string _databaseName = "";
        private string _userName = "";
        private string _password = "";

        public string EndPointUri
        {
            get { return _endpointUri; }
            set { _endpointUri = value; }
        }
        public string StorageKey
        {
            get { return _storageKey; }
            set { _storageKey = value; }
        }
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }
        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        #endregion

        public ImportExportHelper(ILogger log)
        {
            if (log == null) { throw new ArgumentNullException("log"); }

            _log = log;
        }

        public string DoExport(string blobUri)
        {
            _log.Information("Starting Export Operation - {NowTime:T}", DateTime.Now);
            string requestGuid = null;
            bool exportComplete = false;
            string exportedBlobPath = null;

            //Setup Web Request for Export Operation
            WebRequest webRequest = WebRequest.Create(this.EndPointUri + @"/Export");
            webRequest.Method = WebRequestMethods.Http.Post;
            webRequest.ContentType = @"application/xml";

            //Create Web Request Inputs - Blob Storage Credentials and Server Connection Info
            ExportInput exportInputs = new ExportInput
            {
                BlobCredentials = new BlobStorageAccessKeyCredentials
                {
                    StorageAccessKey = this.StorageKey,
                    Uri = String.Format(blobUri, this.DatabaseName, DateTime.UtcNow.Ticks.ToString())
                },
                ConnectionInfo = new ConnectionInfo
                {
                    ServerName = this.ServerName,
                    DatabaseName = this.DatabaseName,
                    UserName = this.UserName,
                    Password = this.Password
                }
            };

            //Perform Web Request
            _log.Information("Making Web Request For Export Operation...");
            Stream webRequestStream = webRequest.GetRequestStream();
            DataContractSerializer dataContractSerializer = new DataContractSerializer(exportInputs.GetType());
            dataContractSerializer.WriteObject(webRequestStream, exportInputs);
            webRequestStream.Close();

            //Get Response and Extract Request Identifier
            _log.Information("Reading Response and extracting Export Request Identifier...");
            WebResponse webResponse = null;
            XmlReader xmlStreamReader = null;

            try
            {
                //Initialize the WebResponse to the response from the WebRequest
                webResponse = webRequest.GetResponse();

                xmlStreamReader = XmlReader.Create(webResponse.GetResponseStream());
                xmlStreamReader.ReadToFollowing("guid");
                requestGuid = xmlStreamReader.ReadElementContentAsString();
                _log.Information("Your Export Request Guid is: {IERequestGuid}", requestGuid);

                //Get Export Operation Status
                while (!exportComplete)
                {
                    _log.Information("Checking export status...");
                    List<StatusInfo> statusInfoList = CheckRequestStatus(requestGuid);
                    _log.Information("{ExportStatus}", statusInfoList.FirstOrDefault().Status);

                    if (statusInfoList.FirstOrDefault().Status == "Failed")
                    {
                        _log.Error("Database export failed: {ExportFailMessage}", statusInfoList.FirstOrDefault().ErrorMessage);
                        exportComplete = true;
                    }

                    if (statusInfoList.FirstOrDefault().Status == "Completed")
                    {
                        exportedBlobPath = statusInfoList.FirstOrDefault().BlobUri;
                        _log.Information("Export Complete - Database exported to: {ExportCompleteUri}", exportedBlobPath);
                        exportComplete = true;
                    }
                }
                return exportedBlobPath;
            }
            catch (WebException responseException)
            {
                _log.Error("Request Falied: {ExportFailMessage}", responseException.Message);
                if (responseException.Response != null)
                {
                    _log.Error("Status Code: {ExportFailMessage}", ((HttpWebResponse)responseException.Response).StatusCode);
                    _log.Error("Status Description: {ExportFailMessage}", ((HttpWebResponse)responseException.Response).StatusDescription);
                }
                throw new Exception(string.Format("AzureExport failed for Database {0} to {1}", DatabaseName, blobUri), responseException);
            }
        }

        public bool DoImport(string blobUri)
        {
            _log.Information("Starting Import Operation - {NowTime:T}", DateTime.Now);
            string requestGuid = null;
            bool importComplete = false;

            //Setup Web Request for Import Operation
            WebRequest webRequest = WebRequest.Create(this.EndPointUri + @"/Import");
            webRequest.Method = WebRequestMethods.Http.Post;
            webRequest.ContentType = @"application/xml";

            //Create Web Request Inputs - Database Size & Edition, Blob Store Credentials and Server Connection Info
            ImportInput importInputs = new ImportInput
            {
                AzureEdition = "Web",
                DatabaseSizeInGB = 1,
                BlobCredentials = new BlobStorageAccessKeyCredentials
                {
                    StorageAccessKey = this.StorageKey,
                    Uri = String.Format(blobUri, this.DatabaseName, DateTime.UtcNow.Ticks.ToString())
                },
                ConnectionInfo = new ConnectionInfo
                {
                    ServerName = this.ServerName,
                    DatabaseName = this.DatabaseName,
                    UserName = this.UserName,
                    Password = this.Password
                }
            };

            //Perform Web Request
            _log.Information("Making Web Request For Import Operation...");
            Stream webRequestStream = webRequest.GetRequestStream();
            DataContractSerializer dataContractSerializer = new DataContractSerializer(importInputs.GetType());
            dataContractSerializer.WriteObject(webRequestStream, importInputs);
            webRequestStream.Close();

            //Get Response and Extract Request Identifier
            _log.Information("Serializing response and extracting guid...");
            WebResponse webResponse = null;
            XmlReader xmlStreamReader = null;

            try
            {
                //Initialize the WebResponse to the response from the WebRequest
                webResponse = webRequest.GetResponse();

                xmlStreamReader = XmlReader.Create(webResponse.GetResponseStream());
                xmlStreamReader.ReadToFollowing("guid");
                requestGuid = xmlStreamReader.ReadElementContentAsString();
                _log.Information("Request Guid: {IERequestGuid}", requestGuid);

                //Get Status of Import Operation
                while (!importComplete)
                {
                    _log.Information("Checking status of Import...");
                    List<StatusInfo> statusInfoList = CheckRequestStatus(requestGuid);
                    _log.Information("{ImportStatus}", statusInfoList.FirstOrDefault().Status);

                    if (statusInfoList.FirstOrDefault().Status == "Failed")
                    {
                        _log.Error("Database import failed: {ImportFailMessage}", statusInfoList.FirstOrDefault().ErrorMessage);
                        importComplete = true;
                    }

                    if (statusInfoList.FirstOrDefault().Status == "Completed")
                    {
                        _log.Information("Import Complete - Database imported to: {ImportCompleteDatabase}", statusInfoList.FirstOrDefault().DatabaseName);
                        importComplete = true;
                    }
                }
                return importComplete;
            }
            catch (WebException responseException)
            {
                _log.Error("Request Falied: {ImportFailMessage}", responseException.Message);
                if (responseException.Response != null)
                {
                    _log.Error("Status Code: {ImportFailMessage}", ((HttpWebResponse)responseException.Response).StatusCode);
                    _log.Error("Status Description: {ImportFailMessage}", ((HttpWebResponse)responseException.Response).StatusDescription);
                }

                throw new Exception(string.Format("AzureImport failed for Database {0} to {1}", DatabaseName, blobUri), responseException);
            }
        }

        public List<StatusInfo> CheckRequestStatus(string requestGuid)
        {
            WebRequest webRequest = WebRequest.Create(this.EndPointUri + string.Format("/Status?servername={0}&username={1}&password={2}&reqId={3}",
                WebUtility.UrlEncode(this.ServerName),
                WebUtility.UrlEncode(this.UserName),
                WebUtility.UrlEncode(this.Password),
                WebUtility.UrlEncode(requestGuid)));

            webRequest.Method = WebRequestMethods.Http.Get;
            webRequest.ContentType = @"application/xml";
            WebResponse webResponse = webRequest.GetResponse();
            XmlReader xmlStreamReader = XmlReader.Create(webResponse.GetResponseStream());
            DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(List<StatusInfo>));

            return (List<StatusInfo>)dataContractSerializer.ReadObject(xmlStreamReader, true);
        }
    }
}