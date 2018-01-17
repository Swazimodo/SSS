using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SSS.Web.Configuration;
using SSS.Web.MiddleWare;

namespace SSS.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Conditionally adds and configures Antiforgery service as per configuration
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <param name="settings">An SSS.Web.Configuration.WebSettingsBase to configure the provided Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions.</param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
        public static IServiceCollection AddAntiforgery(this IServiceCollection services, WebSettingsBase settings)
        {
            if (settings.CSRFSettings.Enabled)
                services.AddAntiforgery(opts =>
                {
                    opts.HeaderName = AntiforgeryHandlerMiddleware.CSRF_HEADER_NAME;
                    opts.FormFieldName = AntiforgeryHandlerMiddleware.CSRF_FORM_FIELD_NAME;
                    opts.Cookie.Name = AntiforgeryHandlerMiddleware.CSRF_COOKIE_NAME;

                    //use same expiration timespan as session
                    opts.Cookie.Expiration = settings.GetSessionExpirationTimeSpan();
                });
            return services;
        }
    }
}
