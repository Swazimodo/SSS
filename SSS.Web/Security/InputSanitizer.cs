using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Web.Security
{
    public static class InputSanitizer
    {
        /// <summary>
        /// Remove any malicious content from user input and preserves line endings
        /// </summary>
        /// <param name="value">multiline input</param>
        /// <returns>HTML Sanitized input</returns>
        public static string SanitizeHTMLMultilineValue(string value)
        {
            string[] arr = value.Split('\n');
            for (int i = 0; i < arr.Length; i++)
                arr[i] = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(arr[i]);
            return string.Join("\n", arr);
        }

        /// <summary>
        /// Removes any malicious content from HTML user input
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SanitizeHTMLValue(string value)
        {
            return Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(value);
        }

        /// <summary>
        /// Allows only [a-zA-Z -]
        /// </summary>
        /// <param name="value">value to test</param>
        /// <returns>true if valid input</returns>
        public static bool ValidateText(string value, int min = 0, int? max = null)
        {
            string pattern = "^[a-zA-Z -]";
            if (min == max)
                pattern += "{" + min.ToString() + "}";
            else
                pattern += "{" + min.ToString() + "," + (max == null ? string.Empty : max.ToString()) + "}";
            pattern += "$";

            return System.Text.RegularExpressions.Regex.IsMatch(value, pattern);
        }
    }
}
