namespace DemoFunctionApp.Services.Contracts
{
    public interface IApplicationConfiguration
    {
        string StorageConnectionString { get; }

        string StorageContainerName { get; }
    }
}