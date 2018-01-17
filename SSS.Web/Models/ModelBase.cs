using System;
using System.Net;
using Microsoft.Extensions.Logging;

namespace SSS.Web.Models
{
    public class ModelBase
    {
        public const string DEFAULT_ERROR_MSG = "There was an issue processing the request";

        public bool HasError { get; set; }
        public string Message { get; set; }

        public ModelBase()
        {
            HasError = false;
            Message = string.Empty;
        }

        /// <summary>
        /// Adds a system error that will be genericized in PROD
        /// </summary>
        public static void AddError(ModelBase model, string error, Configuration.WebSettingsBase settings, ILogger logger)
        {
            model.HasError = true;
            if (settings.ErrorHandlerSettings.ShowErrors)
                model.Message = error;
            else
                model.Message = DEFAULT_ERROR_MSG;

            logger.LogError(error);
        }

        /// <summary>
        /// Adds a system error that will be genericized in PROD
        /// </summary>
        public static void AddError(ModelBase model, Exception error, Configuration.WebSettingsBase settings, ILogger logger)
        {
            model.HasError = true;
            if (settings.ErrorHandlerSettings.ShowErrors)
                model.Message = error.Message;
            else
                model.Message = DEFAULT_ERROR_MSG;
            logger.LogError(new EventId(), error, error.Message);
        }

        /// <summary>
        /// Adds a business error that will be passed back to the client
        /// </summary>
        public static void AddBusinessError(ModelBase model, string message, ILogger logger)
        {
            model.HasError = true;
            model.Message = message;
            logger.LogInformation(message);
        }
    }
}
