namespace DemoFunctionApp.Services
{
    using System;
    using System.Threading.Tasks;
    using System.IO;
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureBlobStorageService(IApplicationConfiguration applicationConfiguration)
        {
            _connectionString = applicationConfiguration.StorageConnectionString;
            _containerName = applicationConfiguration.StorageContainerName;
        }

        private static async Task<Uri> AddNewBlobWithSasAsync(CloudBlobContainer container, Stream blobContents,
            string blobName, IDictionary<string, string> metadata)
        {
            //Get a reference to a blob within the container.
            var blockBlob = container.GetBlockBlobReference(blobName);

            foreach (var data in metadata)
                blockBlob.Metadata.Add(data.Key, data.Value);

            using (blobContents)
                await blockBlob.UploadFromStreamAsync(blobContents).ConfigureAwait(false);

            //Set the expiry time and permissions for the blob.
            //In this case, the start time is specified as a few minutes in the past, to mitigate clock skew.
            //The shared access signature will be valid immediately.
            var sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-1),
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(1),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write
            };

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            var sasBlobToken = blockBlob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return new Uri(blockBlob.Uri + sasBlobToken);
        }

        public async Task<Uri> AddFileAsync(Stream fileStream, string fullFileName,
            IDictionary<string, string> metadata)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(_containerName);

            var blobUriWithSas =
                await AddNewBlobWithSasAsync(container, fileStream, fullFileName, metadata).ConfigureAwait(false);

            return blobUriWithSas;
        }
    }
}