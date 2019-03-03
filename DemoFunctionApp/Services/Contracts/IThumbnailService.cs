namespace DemoFunctionApp.Services.Contracts
{
    using System.IO;

    public interface IThumbnailService
    {
        Stream GenerateThumbnail(Stream originalStream);
    }
}