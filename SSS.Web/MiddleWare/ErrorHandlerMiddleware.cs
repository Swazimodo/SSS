﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SSS.Utilities;
using SSS.Web.Configuration;
using SSS.Web.Extensions;
using SSS.Web.Models;

namespace SSS.Web.MiddleWare
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ErrorHandlerMiddleware
    {
        const string ERROR_COUNT_KEY = "ErrorHandlerMiddleware_ErrorCount";

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ErrorHandlerOptions _options;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger, IOptions<ErrorHandlerOptions> options)
        {
            if (options.Value.WebSettings == null)
                throw new ArgumentNullException("WebSettings", "Unable to get WebSettings from the ErrorHandlerOptions");

            _next = next;
            _logger = logger;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);

                //check to see if the authorize attibute dropped the request
                if (httpContext.Response.StatusCode == 403)
                    throw new AuthorizationException("Access denied");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, _options, _logger);
            }
        }

        /// <summary>
        /// API exceptions will return a JSON error message
        /// Page exceptions will redirect to an error page based on the http status code in not Dev environments
        /// </summary>
        private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception, ErrorHandlerOptions options, ILogger logger)
        {
            //this checks if we should rethrow the error to display in the development exception screen.
            bool throwDevError = false;
            string responseText = "";

            EventId eventID;
            if (exception is BaseException)
                eventID = ((BaseException)exception).EventID;
            else
                eventID = new EventId();

            try
            {
                int httpStatusCode = exception.GetStatusCode();
                httpContext.Response.StatusCode = httpStatusCode;

                //check if this is an API error or a Page error
                if (httpContext.Request.Path.Value.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    //partial views will be expecting HTML but we are returning a JSON error message
                    httpContext.Response.ContentType = "application/json; charset=utf-8";

                    APIError error;
                    if (options.WebSettings.ShowErrors)
                        error = new APIError(exception);
                    else
                        error = new APIError();
                    error.Action = exception.GetAction();

                    //log error details
                    logger.LogError(eventID, exception, error.ReferenceNum.ToString() + " - " + exception.Message);

                    //create a JSON response for API error
                    responseText = error.ToString();
                }
                else
                {
                    //get TempData handle
                    ITempDataDictionaryFactory factory = httpContext.RequestServices.GetService(typeof(ITempDataDictionaryFactory)) as ITempDataDictionaryFactory;
                    ITempDataDictionary tempData = factory.GetTempData(httpContext);

                    //pass reference number to error controller
                    Guid ReferenceNum = Guid.NewGuid();
                    tempData["ReferenceNumber"] = ReferenceNum.ToString();

                    //log error details
                    logger.LogError(eventID, exception, ReferenceNum.ToString() + " - " + exception.Message);

                    //handle Page error
                    if (options.WebSettings.ShowErrors)
                        throwDevError = true;
                    else
                        //if this was a page load redirect to the error page
                        httpContext.Response.Redirect("/Error/" + httpStatusCode.ToString("d"));
                }

                //track number of errors in this session
                int errorCount = httpContext.Session.GetInt32(ERROR_COUNT_KEY).GetValueOrDefault();
                httpContext.Session.SetInt32(ERROR_COUNT_KEY, ++errorCount);
                options.LogErrorCallback?.Invoke(exception, httpContext, options.WebSettings, logger);

                //if the max number is reached block user
                if (options.WebSettings.MaxSessionErrors != null && options.WebSettings.MaxSessionErrors == errorCount)
                    options.MaxErrorCountCallback?.Invoke(httpContext, options.WebSettings, logger);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
                throw ex;
            }

            if (throwDevError)
                throw exception;

            return httpContext.Response.WriteAsync(responseText);
        }
    }
}
