namespace DemoFunctionApp.Services
{
    public interface IApplicationConfiguration
    {
        string StorageConnectionString { get; }
        
        string StorageContainerName { get; }
    }
}
