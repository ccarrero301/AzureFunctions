using System.IO;

namespace DemoFunctionApp.Services.Contracts
{
    public interface IThumbnailService
    {
        Stream GenerateThumbnail(Stream originalStream);
    }
}