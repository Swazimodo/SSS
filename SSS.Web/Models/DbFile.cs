using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SSS.Web.Models
{
    class DbFile
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public byte[] Contents { get; set; }
        public ILogger Logger { get; set; }

        public IActionResult ToClient()
        {
            Logger.LogInformation($"Downloading {FileName}.");

            FileContentResult file = new FileContentResult(Contents, ContentType);
            file.FileDownloadName = FileName;

            return file;
        }

        public IActionResult ToClient(string cache)
        {
            Logger.LogInformation($"Downloading {FileName} with an etag value of {cache}.");

            FileContentResult file = new FileContentResult(Contents, ContentType);
            file.FileDownloadName = FileName;

            Microsoft.Net.Http.Headers.EntityTagHeaderValue etag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue(cache);
            file.EntityTag = etag;

            return file;
        }

        public IActionResult ToClient(DateTime dateModified)
        {
            Logger.LogInformation($"Downloading {FileName}.");

            FileContentResult file = new FileContentResult(Contents, ContentType);
            file.FileDownloadName = FileName;
            file.LastModified = dateModified;

            return file;
        }

        public IActionResult ToClient(DateTime dateModified, string cache)
        {
            Logger.LogInformation($"Downloading {FileName} with an etag value of {cache}.");

            FileContentResult file = new FileContentResult(Contents, ContentType);
            file.FileDownloadName = FileName;
            file.LastModified = dateModified;

            Microsoft.Net.Http.Headers.EntityTagHeaderValue etag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue(cache);
            file.EntityTag = etag;

            return file;
        }
    }
}
