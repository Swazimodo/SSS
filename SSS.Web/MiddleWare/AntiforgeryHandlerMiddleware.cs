using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using SSS.Web.Configuration;

namespace SSS.Web.MiddleWare
{
    public class AntiforgeryHandlerMiddleware
    {
        //use the naming convention expected by Angular.js
        public const string CSRF_HEADER_NAME = "X-XSRF-TOKEN";
        public const string CSRF_COOKIE_NAME = "XSRF-TOKEN";
        public const string CSRF_FORM_FIELD_NAME = "XSRF-TOKEN";

        private readonly RequestDelegate _next;
        private readonly WebSettingsBase _settings;
        private readonly IAntiforgery _antiforgery;

        public AntiforgeryHandlerMiddleware(RequestDelegate next, IOptions<WebSettingsBase> settings, IAntiforgery antiforgery)
        {
            _next = next;
            _settings = settings.Value;
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
                        throw new AntiforgeryValidationException("Request is not valid");
                    await checkValues;
                }
                catch (Exception ex)
                {
                    // wrap in the SSS exception for easier handling later
                    throw new Utilities.Exceptions.AntiforgeryCheckException("Failed to validate CSRF token", ex);
                }
            }

            await _next(httpContext);
        }
    }
}
