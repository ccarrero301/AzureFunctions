namespace DemoFunctionApp.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IFileStorageService
    {
        Task<Uri> AddFileAsync(Stream fileStream, string fullFileName, IDictionary<string, string> metadata);
    }
}
