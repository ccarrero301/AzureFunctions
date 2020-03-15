using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudStorage.Contracts;
using DemoFunctionApp.Services.Contracts;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace DemoFunctionApp.Functions
{
    // http://localhost:7071/runtime/webhooks/EventGrid?functionName=EventGridTriggerFunctionTest
    // https://docs.microsoft.com/en-us/azure/azure-functions/functions-debug-event-grid-trigger-local
    public class EventGridTriggerFunction
    {
        private readonly IThumbnailService _thumbnailService;
        private readonly ICloudFileStorage _fileStorageService;

        public EventGridTriggerFunction(IThumbnailService thumbnailService, ICloudFileStorage fileStorageService)
        {
            _thumbnailService = thumbnailService;
            _fileStorageService = fileStorageService;
        }

        [FunctionName("ThumbnailsFunction")]
        public async Task ThumbnailsFunctionAsync(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [Blob("{data.url}", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream inputImageBlob,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Event Grid Event Topic:\n {eventGridEvent.Topic}");

                log.LogInformation($"Event Grid Event Subject:\n {eventGridEvent.Subject}");

                log.LogInformation($"Event Grid Event Data:\n {eventGridEvent.Data}");

                var originalFileName = eventGridEvent.Subject.Split('/').Last();

                var thumbnailFileName = originalFileName.Insert(originalFileName.IndexOf('.'), "Thumbnail");

                log.LogInformation("Uploading the thumbnail...");

                var thumbnailStream = _thumbnailService.GenerateThumbnail(inputImageBlob);

                var thumbnailUri = await _fileStorageService
                    .UploadStreamAsync(thumbnailStream, $"thumbnails/{thumbnailFileName}", SetMetadata(eventGridEvent))
                    .ConfigureAwait(false);

                log.LogInformation("Thumbnail uploaded...");

                log.LogInformation($"Thumbnail Uri:\n {thumbnailUri}");
            }
            catch (Exception exc)
            {
                log.LogInformation($"Exception Generating or Uploading Image Thumbnail:\n {exc.Message}");

                log.LogInformation($"Exception StackTrace:\n {exc.StackTrace}");

                throw;
            }
        }

        private IDictionary<string, string> SetMetadata(EventGridEvent eventGridEvent) =>
            new Dictionary<string, string>
            {
                {"OriginalFileName", eventGridEvent.Subject.Split('/').Last()},
                {"Subject", eventGridEvent.Subject},
                {"Topic", eventGridEvent.Topic},
                {"EventTime", eventGridEvent.EventTime.ToString(CultureInfo.InvariantCulture)}
            };
    }
}