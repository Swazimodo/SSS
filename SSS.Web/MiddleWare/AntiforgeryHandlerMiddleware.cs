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
        public const string CSRF_HEADER_KEY = "CSRF-HEADER";
        public const string CSRF_COOKIE_KEY = "CSRF-COOKIE";

        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiforgery;

        public AntiforgeryHandlerMiddleware(RequestDelegate next, IAntiforgery antiforgery)
        {
            _next = next;
            _antiforgery = antiforgery;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //only do CSRF checks on the API calls
            if (httpContext.Request.Path.Value.ToLower().StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                //TODO: make this a configuratable list
                //check exclusions list to bypass CSRF check
                if (httpContext.Request.Path.Value.ToLower().StartsWith("/api/v1/Reports/Signature/", StringComparison.OrdinalIgnoreCase))
                {
                    await _next(httpContext);
                    return;
                }

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
                    throw new SSS.Utilities.Exceptions.AntiforgeryValidationException("Failed to validate CSRF token", ex);
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
