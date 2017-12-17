using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

using SSS.Utilities.Exceptions;

namespace SSS.Data
{
    /// <summary>
    /// Allows you to hook a logger up to listen for db messages
    /// </summary>
    public class DatabaseLogger
    {
        public const string DB_MSG = "Database Message - Procedure:{0}, LineNumber:{1}, Server:{2}, Message:{3}";

        ILogger _logger;
        public DatabaseLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Event handler to log db messages as warnings to the ILogger
        /// </summary>
        public void LogDBMessages(object sender, SqlInfoMessageEventArgs e)
        {
            foreach (SqlError error in e.Errors)
                _logger.LogWarning(DB_MSG, error.Procedure, error.LineNumber, error.Server, error.Message);

            //if (!string.IsNullOrEmpty(e.Message))
            //    _logger.LogInformation(e.Message);
        }
    }
}
