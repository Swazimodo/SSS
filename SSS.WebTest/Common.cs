using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using SSS.Web.Configuration;

namespace SSS.WebTest
{
    public class Common
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSettings"></param>
        /// <param name="logger"></param>
        public static void LogErrorCallback(HttpContext context, WebSettingsBase webSettings, ILogger logger)
        {
            //ApplicationSettings settings = webSettings as ApplicationSettings;
            //if (settings == null)
            //    throw new ConfigurationException("ErrorHandlerMiddleware did not provide the correct type of settings");

            //int? loginId = context.Session.GetInt32(Constants.KEY_LOGIN_ID);
            //if (loginId == null)
            //    return;

            //List<SqlParameter> sqlParams = new List<SqlParameter>();
            //sqlParams.Add(new SqlParameter("@loginId", (int)loginId));
            //DataTableResult dt = StoredProcs.GetDataTable(settings.AppDB, Constants.SP_LOG_ERROR, sqlParams, settings.LogDBMessages, logger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSettings"></param>
        /// <param name="logger"></param>
        public static void MaxErrorCountCallback(HttpContext context, WebSettingsBase webSettings, ILogger logger)
        {
            //ApplicationSettings settings = webSettings as ApplicationSettings;
            //if (settings == null)
            //    throw new ConfigurationException("ErrorHandlerMiddleware did not provide the correct type of settings");

            //List<SqlParameter> sqlParams = new List<SqlParameter>();
            //sqlParams.Add(new SqlParameter("@loginName", ADHelper.GetUserName(context)));
            //DataTableResult dt = StoredProcs.GetDataTable(settings.AppDB, Constants.SP_LOCK_ACCOUNT, sqlParams, settings.LogDBMessages, logger);

            ////kill current session
            //ControllerHelper.KillSession(context);
        }
    }
}
