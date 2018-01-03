using System;

namespace SSS.Web.Configuration
{
    /// <summary>
    /// Configuration for the web server setting inherit if you want to add new config values (ex. AppDB)
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
        /// 
        /// </summary>
        public GlobalErrorHandlerSettings ErrorHandlerSettings { get; set; }

        /// <summary>
        /// Whether DB messages are logged
        /// </summary>
        public bool LogDBMessages { get; set; }

        ///// <summary>
        ///// This needs to be enabled in PROD for security but will break swashbuckle swagger UI test forms
        ///// </summary>
        //public bool EnableCSRF { get; set; }

        /// <summary>
        /// Contains all the configuration to enable site CSRF checking
        /// </summary>
        public GlobalCSRFSettings CSRFSettings { get; set; }

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
        /// Maximum number of errors allowed in one session before user is locked out
        /// null value == unlimited
        /// </summary>
        public int? MaxSessionErrors { get; set; }

        public WebSettingsBase()
        {
            //set default values
            AppVersion = "0.0.1";
            EnvironmentName = "Development";
            ErrorHandlerSettings = new GlobalErrorHandlerSettings() { ShowErrors = true };
            //ShowErrors = true;
            LogDBMessages = true;
            CSRFSettings = new GlobalCSRFSettings() { Enabled = false };
            //EnableCSRF = true;
            IdleTimeout = 60; //one hour
            DateFormat = "yyyy-MM-dd";
            MaxSessionErrors = null; //null == unlimited
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
            return string.Compare(EnvironmentName, "Development", true) == 0;
        }

        /// <summary>
        /// Check if this is currently in the production environment
        /// </summary>
        public bool IsProduction()
        {
            return string.Compare(EnvironmentName, "Production", true) == 0;
        }
    }
}
