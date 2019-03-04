namespace DemoFunctionApp.Services.Contracts
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface ICloudFileStorageService
    {
        Task<Uri> UploadStreamAsync(Stream sourceStream, string destinationFullPath, IDictionary<string, string> metaData);

        Task<Stream> DownloadFileAsStreamAsync(string fileFullPath);

        Task<IDictionary<string, string>> GetFileMetadataAsync(string fileFullPath);
    }
}