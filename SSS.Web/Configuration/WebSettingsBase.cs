using System;
using SSS.Utilities.Interfaces;

namespace SSS.Web.Configuration
{
    /// <summary>
    /// Base configuration for the web server. Inherit if you want to add new config values (ex. AppDB)
    /// </summary>
    public class WebSettingsBase
    {
        /// <summary>
        /// Specifies the release version
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Title of HS Environment
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// Session timeout value in minutes
        /// </summary>
        public int IdleTimeout { get; set; }

        /// <summary>
        /// Session cookie expriation timespan (dd.hh:mm:ss)
        /// </summary>
        public string SessionExpiration { private get; set; }

        /// <summary>
        /// Date format to be used throughout site
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Config values for SSS.Data.StoredProc
        /// </summary>
        public StoredProcSettings StoredProcSettings { get; set; }

        /// <summary>
        /// Config values for enabling ErrorHandlerMiddleware
        /// </summary>
        public GlobalErrorHandlerSettings ErrorHandlerSettings { get; set; }

        /// <summary>
        /// Contains all the configuration to enable site CSRF checking middleware
        /// </summary>
        public GlobalCSRFSettings CSRFSettings { get; set; }

        /// <summary>
        /// creates a settings object with default values
        /// </summary>
        public WebSettingsBase()
        {
            //set default values
            AppVersion = "0.0.1";
            EnvironmentName = "Development";
            DateFormat = "yyyy-MM-dd";
            IdleTimeout = 24; //time in minutes
            SessionExpiration = "12:00:00"; //dd.hh:mm:ss

            ErrorHandlerSettings = new GlobalErrorHandlerSettings() {
                ShowErrors = true,
                MaxSessionErrors = null //null == unlimited
            };
            StoredProcSettings = new StoredProcSettings() {
                LogDBMessages = true
            };
            CSRFSettings = new GlobalCSRFSettings() {
                Enabled = false
            };
        }

        /// <summary>
        /// Gets a timespan from the SessionExpiration string
        /// </summary>
        /// <returns>Default to 24 minutes if SessionExpiration null or invalid</returns>
        public TimeSpan? GetSessionExpirationTimeSpan()
        {
            TimeSpan value;
            if (TimeSpan.TryParse(SessionExpiration, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Check if this is currently in the development environment
        /// </summary>
        public bool IsDevelopment()
        {
            return string.Equals(EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if this is currently in the production environment
        /// </summary>
        public bool IsProduction()
        {
            return string.Equals(EnvironmentName, "Production", StringComparison.OrdinalIgnoreCase);
        }
    }
}
