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
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection(ConfigureServices);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IApplicationConfiguration, ApplicationConfiguration>();

            services.AddTransient<IThumbnailService, ThumbnailService>();

            services.AddTransient<ICloudFileStorageService, AzureBlobStorageService>();
        }
    }
}