using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SSS.Web.Configuration;
using SSS.Web.MiddleWare;

namespace SSS.Web.Extensions
{
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds the default DateFormatString to the Newtonsoft.Json.JsonSerializerSettings
        /// </summary>
        /// <param name="mvc">Current MVC settings builder</param>
        /// <param name="settings">An SSS.Web.Configuration.WebSettingsBase to configure the SerializerSettings</param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IMvcBuilder so that additional calls can be chained.</returns>
        public static IMvcBuilder AddSerializerSettings(this IMvcBuilder mvc, WebSettingsBase settings)
        {
            mvc.AddJsonOptions(opts =>
            {
                // configure global date serialization format
                opts.SerializerSettings.DateFormatString = settings.DateFormat;
            });
            return mvc;
        }
    }
}
