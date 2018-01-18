using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ExceptionHandler : Attribute
    {
        public ExceptionOutputMethod OutputMethod { get; private set; }

        public ExceptionHandler(ExceptionOutputMethod method)
        {
            OutputMethod = method;
        }
    }

    public enum ExceptionOutputMethod {
        /// <summary>
        /// For AJAX calls to your API you can return a JSON error object
        /// </summary>
        JSON,

        /// <summary>
        /// For failed page renders use this to redirect to a custom error page
        /// </summary>
        Redirect
    };
}
