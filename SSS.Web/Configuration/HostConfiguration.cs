using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Web.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class HostConfiguration
    {
        /// <summary>
        /// Enable startup error logging
        /// </summary>
        public bool CaptureStartupErrors { get; set; }

        /// <summary>
        /// Current environment name
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Path to site root
        /// </summary>
        public string WebRoot { get; set; }

        /// <summary>
        /// Enables features for local debugging
        /// </summary>
        public bool LocalDebug { get; set; }

        /// <summary>
        /// Create using default options
        /// </summary>
        public HostConfiguration()
        {
            CaptureStartupErrors = true;
            WebRoot = "wwwroot";
            LocalDebug = true;
        }

        /// <summary>
        /// Check if this is currently in the development environment
        /// </summary>
        public bool IsDevelopment()
        {
            return string.Equals(Environment, "Development", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if this is currently in the production environment
        /// </summary>
        public bool IsProduction()
        {
            return string.Equals(Environment, "Production", StringComparison.OrdinalIgnoreCase);
        }
    }
}
