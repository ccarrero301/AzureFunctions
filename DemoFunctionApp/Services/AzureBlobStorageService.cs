namespace DemoFunctionApp.Services
{
    using System;
    using System.Threading.Tasks;
    using System.IO;
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

        private static async Task<Uri> AddNewBlobWithSasAsync(CloudBlobContainer container, Stream blobContents, string blobName)
        {
            //Get a reference to a blob within the container.
            var blob = container.GetBlockBlobReference(blobName);

            using (blobContents)
                await blob.UploadFromStreamAsync(blobContents).ConfigureAwait(false);

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
            var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return new Uri(blob.Uri + sasBlobToken);
        }

        public async Task<Uri> AddFileAsync(Stream fileStream, string fullFileName)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(_containerName);

            var blobUriWithSas = await AddNewBlobWithSasAsync(container, fileStream, fullFileName).ConfigureAwait(false);

            return blobUriWithSas;
        }
    }
}