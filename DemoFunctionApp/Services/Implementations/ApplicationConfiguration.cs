namespace DemoFunctionApp.Services.Implementations
{
    using System;
    using Contracts;

    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string StorageConnectionString => GetConfigurationValue("AzureWebJobsStorage");

        public string StorageContainerName => GetConfigurationValue("StorageContainerName");

        private static string GetConfigurationValue(string value) => Environment.GetEnvironmentVariable(value);
    }
}