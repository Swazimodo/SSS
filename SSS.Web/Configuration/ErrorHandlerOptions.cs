using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SSS.Web.Configuration
{
    public class ErrorHandlerOptions
    {
        public delegate void ErrorHandlerCallback(Exception exception, HttpContext context, GlobalErrorHandlerSettings settings, ILogger logger);
        public delegate void MaxErrorHandlerCallback(HttpContext context, GlobalErrorHandlerSettings settings, ILogger logger);

        /// <summary>
        /// Here you can add custom logging or error handling
        /// </summary>
        public ErrorHandlerCallback LogErrorCallback { get; set; }

        /// <summary>
        /// This is a callback when a user passes a threshold value set to see if one user is having too many errors.
        /// This could indicate an application availablity issue or an intentional attack.
        /// </summary>
        public MaxErrorHandlerCallback MaxErrorCountCallback { get; set; }

        /// <summary>
        /// Global error handler settings, cannot be null
        /// </summary>
        public GlobalErrorHandlerSettings WebSettings { get; set; }
    }
}
