using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DemoFunctionApp.Services.Contracts;
using DemoFunctionApp.Services.Utils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.DataMovement;

namespace DemoFunctionApp.Services.Implementations
{
    public class AzureBlobStorageService : ICloudFileStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureBlobStorageService(IApplicationConfiguration applicationConfiguration)
        {
            _connectionString = applicationConfiguration.StorageConnectionString;
            _containerName = applicationConfiguration.StorageContainerName;
        }

        public async Task<Uri> UploadStreamAsync(Stream sourceStream, string destinationFullPath, IDictionary<string, string> metaData)
        {
            var blockBlobReference = GetBlockBlobReference(destinationFullPath);

            await UploadBlobAsync(blockBlobReference, sourceStream, metaData).ConfigureAwait(false);

            return SasUtils.GetBlobSasUri(blockBlobReference);
        }

        public async Task<Stream> DownloadFileAsStreamAsync(string fileFullPath)
        {
            var blockBlob = GetSasCloudBlockBlob(fileFullPath);

            var targetStream = new MemoryStream();

            await blockBlob.DownloadToStreamAsync(targetStream).ConfigureAwait(false);

            return targetStream;
        }

        public async Task<IDictionary<string, string>> GetFileMetadataAsync(string fileFullPath)
        {
            var blockBlob = GetSasCloudBlockBlob(fileFullPath);

            await blockBlob.FetchAttributesAsync().ConfigureAwait(false);

            return blockBlob.Metadata;
        }

        private CloudBlobContainer GetBlobContainerReference()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            return blobClient.GetContainerReference(_containerName);
        }

        private CloudBlockBlob GetSasCloudBlockBlob(string fileFullPath)
        {
            var blockBlobReference = GetBlockBlobReference(fileFullPath);

            var blockBlobUri = SasUtils.GetBlobSasUri(blockBlobReference);

            var blockBlob = new CloudBlockBlob(blockBlobUri);

            return blockBlob;
        }

        private CloudBlockBlob GetBlockBlobReference(string fileFullPath)
        {
            var blobContainerReference = GetBlobContainerReference();

            return blobContainerReference.GetBlockBlobReference(fileFullPath);
        }

        private static Task UploadBlobAsync(CloudBlob blob, Stream blobContents, IDictionary<string, string> metaData)
        {
            var options = new UploadOptions();

            var context = new SingleTransferContext
            {
                SetAttributesCallbackAsync = async destination =>
                {
                    var destinationBlob = destination as CloudBlockBlob;

                    foreach (var data in metaData)
                        destinationBlob?.Metadata.Add(data.Key, data.Value);
                },

                ShouldOverwriteCallbackAsync = TransferContext.ForceOverwrite
            };

            return TransferManager.UploadAsync(blobContents, blob, options, context);
        }
    }
}