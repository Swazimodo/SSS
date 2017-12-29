using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Web.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// checks if string is "true"
        /// </summary>
        /// <param name="value">string to try and parse</param>
        /// <returns>boolean value</returns>
        /// <exception cref="FormatException">If invalid string</exception>
        public static bool IsTrue(this string value)
        {
            bool b;
            if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out b))
                return b;
            else
                throw new FormatException("Invalid boolean value");
        }
    }
}
