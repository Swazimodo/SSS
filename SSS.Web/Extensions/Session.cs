using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace SSS.Web.Extensions
{
    public static class Session
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
    }
}
