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
        /// Whether errors are passed back through the API
        /// </summary>
        public bool ShowErrors { get; set; }

        /// <summary>
        /// Whether DB messages are logged
        /// </summary>
        public bool LogDBMessages { get; set; }

        /// <summary>
        /// This needs to be enabled in PROD for security but will break swagger UI
        /// </summary>
        public bool EnableCSRF { get; set; }

        /// <summary>
        /// Session timeout value in minutes
        /// </summary>
        public int IdleTimeout { get; set; }

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
            ShowErrors = true;
            LogDBMessages = true;
            EnableCSRF = true;
            IdleTimeout = 60; //one hour
            DateFormat = "yyyy-MM-dd";
            MaxSessionErrors = null; //null == unlimited
        }
    }
}
