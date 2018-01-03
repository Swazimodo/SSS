using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SSS.Web.Configuration
{
    public class CSRFPath
    {
        /// <summary>
        /// Specify HttpMethods if you want to filter by them
        /// </summary>
        public List<string> Methods { get; set; }

        /// <summary>
        /// The relative site page to check 
        /// </summary>
        /// <example>/api/</example>
        public string StartingRequestPath { get; set; }

        /// <summary>
        /// checks if the current request matches the method and path
        /// </summary>
        /// <param name="context">Current context to evaluate</param>
        /// <returns>true if current request matches this rule</returns>
        public bool Validate(HttpContext context)
        {
            bool match = false;

            //check if we will look for a method match
            if (Methods == null || Methods.Count == 0)
                match = true;
            else
            {
                for (int i = 0; i < Methods.Count; i++)
                    if (string.Compare(Methods[i], context.Request.Method, true) == 0)
                    {
                        match = true;
                        break;
                    }
            }

            if (match && context.Request.Path.Value.StartsWith(StartingRequestPath, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}
