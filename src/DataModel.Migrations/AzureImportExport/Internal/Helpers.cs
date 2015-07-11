using System;
using System.Collections.Generic;
using System.Threading;
using EFDDD.DataModel.Migrations.AzureImportExport.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EFDDD.DataModel.Migrations.AzureImportExport.Internal
{
    internal static class Helpers
    {
        internal static IDictionary<Endpoint, string> GetEndpointUris()
        {
            return new Dictionary<Endpoint, string>
            {
                {Endpoint.NorthCentralUS, "https://ch1prod-dacsvc.azure.com/DACWebService.svc"},
                {Endpoint.SouthCentralUS, "https://sn1prod-dacsvc.azure.com/DACWebService.svc"},
                {Endpoint.NorthEurope, "https://db3prod-dacsvc.azure.com/DACWebService.svc"},
                {Endpoint.WestEurope, "https://am1prod-dacsvc.azure.com/DACWebService.svc"},
                {Endpoint.EastAsia, "https://hkgprod-dacsvc.azure.com/DACWebService.svc"},
                {Endpoint.SoutheastAsia, "https://sg1prod-dacsvc.azure.com/DACWebService.svc"},
                {Endpoint.EastUS, "https://bl2prod-dacsvc.azure.com/DACWebService.svc"},
                {Endpoint.WestUS, "https://by1prod-dacsvc.azure.com/DACWebService.svc"}
            };
        }

        internal static CloudStorageAccount GetBlobAccount(string accountName, string accessKey, bool useHttps = true)
        {
            return new CloudStorageAccount(new StorageCredentials(accountName, accessKey), useHttps);
        }

        internal static bool CreateBlobContainerIfNotExists(CloudStorageAccount storageAccount, string containerName, bool isPublic = true)
        {
            // Create the blob client, which provides
            // authenticated access to the Blob service.
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Get the container reference.
            var blobContainer = blobClient.GetContainerReference(containerName);
            // Create the container if it does not exist.
            blobContainer.CreateIfNotExists();

            if (isPublic)
            {
                //// Set permissions on the container.
                var containerPermissions = blobContainer.GetPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Container;
                blobContainer.SetPermissions(containerPermissions);
            }

            return true;
        }

        internal static TResult Retry<TResult>(Func<TResult> func, int maxRetries, int delayInMilliseconds = 0, Action<Exception> onException = null)
        {
            var returnValue = default(TResult);
            var numTries = 0;
            var succeeded = false;

            while (numTries < maxRetries)
            {
                try
                {
                    returnValue = func();
                    succeeded = true;
                }
                catch (Exception ex)
                {
                    if (onException != null)
                    {
                        onException(ex);
                    }
                }
                finally
                {
                    numTries++;
                }
                if (succeeded) { return returnValue; }

                Thread.Sleep(delayInMilliseconds);
            }
            return default(TResult);
        }

        internal static void Retry(Action func, int maxRetries, int delayInMilliseconds = 0, Action<Exception> onException = null)
        {
            var numTries = 0;
            var succeeded = false;

            while (numTries < maxRetries)
            {
                try
                {
                    func();
                    succeeded = true;
                }
                catch (Exception ex)
                {
                    if (onException != null)
                    {
                        onException(ex);
                    }
                }
                finally
                {
                    numTries++;
                }
                if (succeeded) { return; }

                Thread.Sleep(delayInMilliseconds);
            }
        }


    }
}