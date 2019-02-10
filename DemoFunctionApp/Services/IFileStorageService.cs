namespace DemoFunctionApp.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IFileStorageService
    {
        Task<Uri> AddFileAsync(Stream fileStream, string fullFileName);
    }
}
