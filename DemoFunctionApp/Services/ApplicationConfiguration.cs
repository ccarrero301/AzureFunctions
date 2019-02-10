namespace DemoFunctionApp.Services
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;

    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string StorageConnectionString => GetConfigurationValue("StorageConnectionString");

        public string StorageContainerName => GetConfigurationValue("StorageContainerName");

        private static string GetConfigurationValue(string value) => Environment.GetEnvironmentVariable(value);

    }
}