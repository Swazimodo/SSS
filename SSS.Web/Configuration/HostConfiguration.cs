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
        public bool CaptureStartupErrors { get; set; }
        public string Environment { get; set; }
        public string WebRoot { get; set; }

        public HostConfiguration()
        {
            WebRoot = "wwwroot";
        }
    }
}
