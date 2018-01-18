using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
            if (settings.EnableCSRFChecking)
                services.AddAntiforgery(opts =>
                {
                    opts.HeaderName = "X-XSRF-TOKEN";
                    opts.FormFieldName = "XSRF-TOKEN";
                    opts.Cookie.Name = "XSRF-TOKEN";

                    //use same expiration timespan as session
                    opts.Cookie.Expiration = settings.GetSessionExpirationTimeSpan();
                });
            return services;
        }

        /// <summary>
        /// Configures MVC service based on the configuration from WebSettingsBase
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IMvcBuilder AddMvc(this IServiceCollection services, WebSettingsBase settings)
        {
            return services.AddMvc(options => 
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
            ).AddSerializerSettings(settings);
        }
    }
}
