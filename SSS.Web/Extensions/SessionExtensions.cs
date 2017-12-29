using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

using SSS.Utilities.Exceptions;

namespace SSS.Web.Extensions
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Deserialize object out of the session
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="session">current session</param>
        /// <param name="key">session key</param>
        /// <returns>object out of session</returns>
        public static T GetObject<T>(this ISession session, string key) where T : class
        {
            string s = session.GetString(key);
            if (string.IsNullOrEmpty(s))
                return null;

            T obj = JsonConvert.DeserializeObject<T>(s);
            return obj;
        }

        /// <summary>
        /// Serialize object and save it into the session
        /// </summary>
        /// <typeparam name="T">type to serialize</typeparam>
        /// <param name="session">current session</param>
        /// <param name="key">session key</param>
        /// <param name="obj">target object to save a copy of into the session</param>
        public static void SetObject<T>(this ISession session, string key, T obj) where T : class
        {
            string s = JsonConvert.SerializeObject(obj);
            session.SetString(key, s);
        }

        /// <summary>
        /// removes all session values
        /// overwrites asp session cookie
        /// overwrites CSRF Cookie
        /// </summary>
        public static void KillSession(this HttpContext context)
        {
            context.Session.Clear();
            context.Response.Cookies.Append(".AspNetCore.Session", "");
            context.Response.Cookies.Append(MiddleWare.AntiforgeryHandlerMiddleware.CSRF_COOKIE_KEY, "");
        }

        /// <summary>
        /// Gets the client's IPv4 Address
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>IP address</returns>
        public static string GetClientIPAddress(this HttpContext context)
        {
            string ip;

            //check for new forwarded for header
            //https://en.wikipedia.org/wiki/X-Forwarded-For
            //https://stackoverflow.com/questions/6316796/read-x-forwarded-for-header/43554000#43554000
            ip = context.Request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrWhiteSpace(ip))
            {
                string[] IPs = ip.Split(',');
                if (IPs.Length > 1)
                    ip = IPs[0].Trim();
            }

            //check context for remote ip address
            if (string.IsNullOrWhiteSpace(ip) && context.Connection.RemoteIpAddress != null)
            {
                ip = context.Connection.RemoteIpAddress.ToString();
                if (string.IsNullOrWhiteSpace(ip) || ip.Count(x => x == '.') != 3)
                    ip = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }

            //check REMOTE_ADDR header
            if (string.IsNullOrWhiteSpace(ip) || ip.Count(x => x == '.') != 3)
                ip = context.Request.Headers["REMOTE_ADDR"];

            if (string.IsNullOrWhiteSpace(ip) || ip.Count(x => x == '.') != 3)
                throw new BaseException("Unable to determine caller's IP");

            return ip;
        }
    }
}
