using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DemoFunctionApp.Services.Contracts
{
    public interface ICloudFileStorageService
    {
        Task<Uri> UploadStreamAsync(Stream sourceStream, string destinationFullPath, IDictionary<string, string> metaData);

        Task<Stream> DownloadFileAsStreamAsync(string fileFullPath);

        Task<IDictionary<string, string>> GetFileMetadataAsync(string fileFullPath);
    }
}