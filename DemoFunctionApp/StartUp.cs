using CloudStorage.Contracts;
using CloudStorage.Implementations.Azure;
using DemoFunctionApp;
using DemoFunctionApp.Services.Contracts;
using DemoFunctionApp.Services.Implementations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DemoFunctionApp
{
    public class Startup : FunctionsStartup
    { 
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var applicationConfiguration = new ApplicationConfiguration();

            builder.Services.AddScoped<IThumbnailService, ThumbnailService>();

            builder.Services.AddScoped<ICloudFileStorage, BlockBlobStorage>(provider =>
                new BlockBlobStorage(applicationConfiguration.StorageConnectionString, applicationConfiguration.StorageContainerName));
        }
    }
}