using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

using SSS.Web.Configuration;
using SSS.Web.MiddleWare;

namespace SSS.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Insert a global error handler into the pipeline
        /// </summary>
        /// <param name="options">options.WebSettings cannot be null</param>
        /// <returns></returns>
        public static IApplicationBuilder UseErrorHandlerMiddleware(this IApplicationBuilder app, ErrorHandlerOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            //use dev page if we are returning detailed errors
            if (options.WebSettings.ShowErrors)
                app.UseDeveloperExceptionPage();

            return app.UseMiddleware<ErrorHandlerMiddleware>(Options.Create(options));
        }

        /// <summary>
        /// sets status code to 404 if a get request returns with no content
        /// </summary>
        public static IApplicationBuilder UseHttpNoContentOutputMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpNoContentOutputMiddleware>();
        }

        /// <summary>
        /// Insert a global error CSRF checking into the pipeline
        /// </summary>
        public static IApplicationBuilder UseAntiforgeryHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiforgeryHandlerMiddleware>();
        }
    }
}
