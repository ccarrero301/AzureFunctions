namespace DemoFunctionApp.Functions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.EventGrid.Models;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.EventGrid;
    using Microsoft.Extensions.Logging;
    using Services;
    using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

    // http://localhost:7071/runtime/webhooks/EventGrid?functionName=EventGridTriggerFunctionTest
    // https://docs.microsoft.com/en-us/azure/azure-functions/functions-debug-event-grid-trigger-local
    public static class EventGridTriggerFunction
    {
        [FunctionName("ThumbnailsFunction")]
        public static async Task ThumbnailsFunctionAsync(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [Blob("{data.url}", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream inputImageBlob,
            [Inject] IThumbnailService thumbnailService,
            [Inject] IFileStorageService fileStorageService,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Event Grid Event Topic:\n {eventGridEvent.Topic}");

                log.LogInformation($"Event Grid Event Subject:\n {eventGridEvent.Subject}");

                log.LogInformation($"Event Grid Event Data:\n {eventGridEvent.Data}");

                var originalFileName = eventGridEvent.Subject.Split('/').Last();

                log.LogInformation($"Uploading the thumbnail...");

                var thumbnailStream = thumbnailService.GenerateThumbnail(inputImageBlob);

                var thumbnailUri = await fileStorageService
                    .AddFileAsync(thumbnailStream, $"thumbnails/{originalFileName}")
                    .ConfigureAwait(false);

                log.LogInformation($"Thumbnail uploaded...");

                log.LogInformation($"Thumbnail Uri:\n {thumbnailUri}");
            }
            catch (Exception exc)
            {
                log.LogInformation($"Exception Generating or Uploading Image Thumbnail:\n {exc.Message}");

                log.LogInformation($"Exception StackTrace:\n {exc.StackTrace}");

                throw;
            }
        }
    }
}