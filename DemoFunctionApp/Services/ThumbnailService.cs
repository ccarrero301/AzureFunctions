﻿namespace DemoFunctionApp.Services
{
    using System.IO;
    using PhotoSauce.MagicScaler;

    public class ThumbnailService : IThumbnailService
    {
        public Stream GenerateThumbnail(Stream originalStream)
        {
            var outputStream = new MemoryStream();

            MagicImageProcessor.ProcessImage(originalStream, outputStream, new ProcessImageSettings { Width = 50, Height = 50 });

            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }
    }
}