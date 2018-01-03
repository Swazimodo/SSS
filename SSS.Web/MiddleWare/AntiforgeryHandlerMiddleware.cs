using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SSS.Web.MiddleWare
{
    public class AntiforgeryHandlerMiddleware
    {
        //use the naming convention expected by Angular.js
        public const string CSRF_HEADER_KEY = "X-XSRF-TOKEN";
        public const string CSRF_COOKIE_KEY = "XSRF-TOKEN";

        private readonly RequestDelegate _next;
        private readonly Configuration.WebSettingsBase _settings;
        private readonly IAntiforgery _antiforgery;

        public AntiforgeryHandlerMiddleware(RequestDelegate next, Configuration.WebSettingsBase settings, IAntiforgery antiforgery)
        {
            _next = next;
            _settings = settings;
            _antiforgery = antiforgery;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if(_settings.CSRFSettings.Enabled == false)
            {
                await _next(httpContext);
                return;
            }

            //only do CSRF checks on paths specified in the configuration
            if (_settings.CSRFSettings.isCSRFCheckNeeded(httpContext))
            {
                try
                {
                    Task checkValues = _antiforgery.ValidateRequestAsync(httpContext);
                    Task<bool> validate = _antiforgery.IsRequestValidAsync(httpContext);
                    if (!(await validate))
                        throw new Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException("Request is not valid");
                    await checkValues;
                }
                catch (Exception ex)
                {
                    // wrap in the SSS exception for easier handling later
                    throw new SSS.Utilities.Exceptions.AntiforgeryCheckException("Failed to validate CSRF token", ex);
                }
            }

            await _next(httpContext);
        }
    }

    /// <summary>
    /// Extension method used to add the error handling middleware to the HTTP request pipeline
    /// </summary>
    public static class AntiforgeryHandlerMiddlewareExtensions
    {
        /// <summary>
        /// Checks config to see if it should be enabled
        /// </summary>
        public static IApplicationBuilder UseAntiforgeryHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiforgeryHandlerMiddleware>();
        }
    }
}
