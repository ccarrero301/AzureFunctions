namespace DemoFunctionApp.Services.Utils
{
    using System;
    using Microsoft.WindowsAzure.Storage.Blob;

    public static class SasUtils
    {
        public static Uri GetBlobSasUri(CloudBlob blockBlob)
        {
            //Set the expiry time and permissions for the blob.
            //In this case, the start time is specified as a few minutes in the past, to mitigate clock skew.
            var sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-1),
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(10),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write
            };

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            var sasBlobToken = blockBlob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the blob, including the SAS token.
            return new Uri(blockBlob.Uri + sasBlobToken);
        }
    }
}