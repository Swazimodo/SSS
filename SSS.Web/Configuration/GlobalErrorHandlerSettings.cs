using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Web.Configuration
{
    /// <summary>
    /// Config values for enabling ErrorHandlerMiddleware
    /// </summary>
    public class GlobalErrorHandlerSettings
    {
        /// <summary>
        /// Whether errors are passed back through the API
        /// </summary>
        public bool ShowErrors { get; set; }

        /// <summary>
        /// Will redirect users to this page if a page request threw and exception
        /// </summary>
        /// <example>/Error/</example>
        public string ErrorPage { get; set; }

        /// <summary>
        /// Any paths referenced here will have JSON error details returned instead of firing a redict to the error page
        /// </summary>
        /// <example>"/api/", "/websocket/"</example>
        public List<string> JsonErrorPaths { get; set; }
        
        /// <summary>
        /// Maximum number of errors allowed in one session before user is locked out
        /// null value == unlimited
        /// </summary>
        public int? MaxSessionErrors { get; set; }

        public GlobalErrorHandlerSettings()
        {
            JsonErrorPaths = new List<string>();
        }
    }
}
