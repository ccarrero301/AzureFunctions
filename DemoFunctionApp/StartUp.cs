[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(DemoFunctionApp.Startup))]

namespace DemoFunctionApp
{
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.DependencyInjection;
    using Services.Contracts;
    using Services.Implementations;
    using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder) =>
            builder.AddDependencyInjection(ConfigureServices);

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IApplicationConfiguration, ApplicationConfiguration>();

            services.AddTransient<IThumbnailService, ThumbnailService>();

            services.AddTransient<ICloudFileStorageService, AzureBlobStorageService>();
        }
    }
}