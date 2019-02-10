namespace DemoFunctionApp.Services
{
    using System.IO;

    public interface IThumbnailService
    {
        Stream GenerateThumbnail(Stream originalStream);
    }
}
