using System;
using System.Net;

using SSS.Web.Models;
using SSS.Utilities.Enums;
using SSS.Utilities.Exceptions;

namespace SSS.Web.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Get the requried client action for each type of error
        /// </summary>
        /// <param name="exception">Exception thrown</param>
        /// <returns>client action</returns>
        public static string GetAction(this Exception exception)
        {
            string action = ErrorAction.Support.ToString();

            //check if error has an easy solution
            if (exception is AntiforgeryCheckException) action = ErrorAction.Reload.ToString();
            else if (exception is SessionException) action = ErrorAction.Reload.ToString();

            return action;
        }

        public static string ToJSONString(this Exception exception)
        {
            return (new APIError(exception)).ToString();
        }

        /// <summary>
        /// looks for our custom exception types and maps them to HTTPCodes
        /// </summary>
        /// <returns>HTTPCode</returns>
        public static int GetStatusCode(this Exception exception)
        {
            //check for custom exceptions
            HttpStatusCode code = HttpStatusCode.InternalServerError; // 500 if unexpected
            if (exception is ConfigurationException) code = HttpStatusCode.InternalServerError;
            else if (exception is DataAccessException) code = HttpStatusCode.InternalServerError;
            else if (exception is DataBindException) code = HttpStatusCode.InternalServerError;
            else if (exception is StoredProcedureException) code = HttpStatusCode.InternalServerError;
            else if (exception is NotFoundException) code = HttpStatusCode.NotFound;
            else if (exception is AuthenticationException) code = HttpStatusCode.Unauthorized;
            //if (exception is AntiforgeryValidationException) code = HttpStatusCode.Unauthorized;
            else if (exception is AuthorizationException) code = HttpStatusCode.Forbidden;

            //set status code
            return (int)code;
        }
    }
}
