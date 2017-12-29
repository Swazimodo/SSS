using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SSS.Web.Models
{
    /// <summary>
    /// Used when searving up a file from a database blob
    /// </summary>
    class DbFile
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Contents { get; set; }
        public string ETag { get; set; }
        public DateTime? DateModified { get; set; }
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Gets the extension from FileName property
        /// </summary>
        public string Extension
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FileName))
                    throw new ArgumentException("FileName is not valid");

                int i = FileName.LastIndexOf('.');
                if (i == -1)
                    return string.Empty;

                return FileName.Substring(i + 1);
            }
        }

        /// <summary>
        /// Creates a file from a DB blob
        /// </summary>
        /// <param name="logger">Adds the ability to track client downloads</param>
        public DbFile(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("Need a valid logger to add the ability to track client downloads");
            Logger = logger;
        }

        /// <summary>
        /// Sends file to the client
        /// </summary>
        public IActionResult ToClient()
        {
            //log what file is attempting to be downloaded
            if (string.IsNullOrWhiteSpace(ETag))
                Logger.LogInformation($"Downloading {FileName}.");
            else
                Logger.LogInformation($"Downloading {FileName} with an etag value of {ETag}.");

            //validate requrired properties
            if (string.IsNullOrWhiteSpace(FileName))
                throw new ArgumentException("Invalid file name");
            if (Contents == null || Contents.Length == 0)
                throw new ArgumentException("File contents null or empty");

            //create IActionResult
            FileContentResult file = new FileContentResult(Contents, ContentType);
            file.FileDownloadName = FileName;

            if (DateModified != null)
                file.LastModified = DateModified;
            if (!string.IsNullOrWhiteSpace(ETag))
            {
                Microsoft.Net.Http.Headers.EntityTagHeaderValue etag = 
                    new Microsoft.Net.Http.Headers.EntityTagHeaderValue(ETag);
                file.EntityTag = etag;
            }

            return file;
        }
    }
}
