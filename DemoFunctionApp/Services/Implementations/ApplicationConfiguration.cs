using System;
using DemoFunctionApp.Services.Contracts;

namespace DemoFunctionApp.Services.Implementations
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string StorageConnectionString => GetConfigurationValue("AzureWebJobsStorage");

        public string StorageContainerName => GetConfigurationValue("StorageContainerName");

        private static string GetConfigurationValue(string value)
        {
            return Environment.GetEnvironmentVariable(value);
        }
    }
}