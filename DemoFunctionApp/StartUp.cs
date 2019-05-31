using CloudStorage.Contracts;
using CloudStorage.Implementations.Azure;
using DemoFunctionApp;
using DemoFunctionApp.Services.Contracts;
using DemoFunctionApp.Services.Implementations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]

namespace DemoFunctionApp
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder) => builder.AddDependencyInjection(ConfigureServices);

        private void ConfigureServices(IServiceCollection services)
        {
            var applicationConfiguration = new ApplicationConfiguration();

            services.AddTransient<IThumbnailService, ThumbnailService>();

            services.AddTransient<ICloudFileStorage, BlockBlobStorage>(provider =>
                new BlockBlobStorage(applicationConfiguration.StorageConnectionString, applicationConfiguration.StorageContainerName));
        }
    }
}