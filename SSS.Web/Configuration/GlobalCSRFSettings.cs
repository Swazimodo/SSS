using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SSS.Web.Configuration
{
    public class GlobalCSRFSettings
    {
        /// <summary>
        /// True if CSRF checks should be enabled for the site
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// list of site paths that should be covered by the CSRF check
        /// </summary>
        public List<CSRFPath> IncludedEndpoints { get; set; }

        /// <summary>
        /// list of site paths that need to be excluded
        /// </summary>
        public List<CSRFPath> ExcludedEndpoints { get; set; }


        public GlobalCSRFSettings()
        {
            ExcludedEndpoints = new List<CSRFPath>();
            IncludedEndpoints = new List<CSRFPath>();
        }

        /// <summary>
        /// Checks if the current request needs to have the CSRF token validated
        /// </summary>
        /// <param name="context">Current context to evaluate</param>
        /// <returns>true if the current request needs to be validated</returns>
        public bool isCSRFCheckNeeded(HttpContext context)
        {
            bool check = false;

            //check if the current request should be included in the CSRF check
            for (int i = 0; i < IncludedEndpoints.Count; i++)
            {
                if (IncludedEndpoints[i].Validate(context))
                {
                    check = true;
                    break;
                }
            }

            //if the request did not match a included path there is no point to check the excluded list
            if (check == false)
                return check;

            //check if the current request should be excluded from the CSRF check
            for (int i = 0; i < ExcludedEndpoints.Count; i++)
            {
                if (ExcludedEndpoints[i].Validate(context))
                {
                    check = false;
                    break;
                }
            }

            return check;
        }
    }
}
