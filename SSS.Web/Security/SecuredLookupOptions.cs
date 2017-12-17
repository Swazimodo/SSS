using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Http;

using SSS.Utilities.Interfaces;
using SSS.Utilities.Exceptions;

namespace SSS.Web.Security
{
    /// <summary>
    /// This helper has a collection of methods to protect making direct object references of sensitive information
    /// https://www.owasp.org/index.php/Top_10_2010-A4-Insecure_Direct_Object_References
    /// </summary>
    public static class SecuredLookupOptions
    {
        //parameter name prefix to prevent accidental overlap
        const string OPTIONS_NAME_PREFIX = "QryStrOpts_";
        const int MAX_KEY_LENGTH = 8;

        /// <summary>
        /// Creates a new QueryStringOptions object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isOneTime">If true object will be disposed after first time retrieving a value</param>
        /// <returns>Options object that has not yet been saved in the session</returns>
        public static SecuredLookupOptions<T> CreateQueryStringParameter<T>(bool isOneTime = false)
        {
            SecuredLookupOptions<T> options = new SecuredLookupOptions<T>(isOneTime);
            return options;
        }

        /// <summary>
        /// Stores values for later retrieval
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="list">object that stores all possible values for this parameter</param>
        /// <param name="value">item to save</param>
        /// <returns>reference key used to lookup value</returns>
        public static string AddQueryStringOption<T>(this SecuredLookupOptions<T> obj, T value)
        {
            //string key = Guid.NewGuid().ToString();
            string key = GetUniqueKey(MAX_KEY_LENGTH);
            while (obj.Options.ContainsKey(key))
                key = GetUniqueKey(MAX_KEY_LENGTH);

            obj.Options.Add(key, value);
            return key;
        }

        /// <summary>
        /// Gets all possible options out of the session and then lookup a particular value
        /// </summary>
        /// <param name="context">user context</param>
        /// <param name="paramName">query string value name. This needs to be unique for your page</param>
        /// <param name="key">reference string used to find value</param>
        /// <param name="allowNull">should null key be an error, if not it counts as a lookup for onetime values</param>
        /// <returns>saved session int</returns>
        public static int GetQueryStringValue(HttpContext context, string paramName, string key, bool allowNull = false)
        {
            return GetQueryStringValue<int>(context, paramName, key, allowNull);
        }

        /// <summary>
        /// Gets all possible options out of the session and then lookup a particular value
        /// </summary>
        /// <typeparam name="T">type of value saved</typeparam>
        /// <param name="context">user context</param>
        /// <param name="paramName">query string value name. This needs to be unique for your page</param>
        /// <param name="key">reference string used to find value</param>
        /// <param name="allowNull">should null key be an error, if not it counts as a lookup for onetime values</param>
        /// <returns>saved session object</returns>
        public static T GetQueryStringValue<T>(HttpContext context, string paramName, string key, bool allowNull = false)
        {
            if (!allowNull && string.IsNullOrWhiteSpace(key))
                throw new ValidationException("Not allowed Null " + paramName + " lookup");

            SecuredLookupOptions<T> qso = context.Session.GetObject<SecuredLookupOptions<T>>(OPTIONS_NAME_PREFIX + paramName);
            if (qso == null)
                throw new SessionException("Unable to find secured options in the session");

            T value;
            //if the lookup was null return default T
            if (string.IsNullOrWhiteSpace(key))
                value = default(T);
            else
            {
                if (!qso.Options.ContainsKey(key))
                    throw new SessionException("Unable to find key in secured options list");
                value = qso.Options[key];
            }

            // remove if this is only allowed to be used once
            if (qso.IsOneTime)
                RemoveQueryStringOptionsList(context, paramName);

            return value;
        }

        /// <summary>
        /// retrieve entire options list from session
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="context">user context</param>
        /// <param name="paramName">query string value name. This needs to be unique for your page</param>
        /// <returns>options list</returns>
        public static SecuredLookupOptions<T> GetQueryStringOptionsList<T>(HttpContext context, string paramName)
        {
            SecuredLookupOptions<T> qso = context.Session.GetObject<SecuredLookupOptions<T>>(OPTIONS_NAME_PREFIX + paramName);
            return qso;
        }

        /// <summary>
        /// Save an options object into the session once all values have been added
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="context">user context</param>
        /// <param name="paramName">query string value name. This needs to be unique for your page</param>
        /// <param name="queryStringOptions">Options object to save into the session</param>
        public static void SaveQueryStringOptionsList<T>(HttpContext context, string paramName, SecuredLookupOptions<T> queryStringOptions)
        {
            context.Session.SetObject(OPTIONS_NAME_PREFIX + paramName, queryStringOptions);
        }

        /// <summary>
        /// removes a QueryStringOptions from the session
        /// </summary>
        /// <param name="context">user context</param>
        /// <param name="paramName">query string value name. This needs to be unique for your page</param>
        public static void RemoveQueryStringOptionsList(HttpContext context, string paramName)
        {
            context.Session.Remove(OPTIONS_NAME_PREFIX + paramName);
        }

        /// <summary>
        /// Generates a random string of charactors
        /// </summary>
        /// <param name="maxSize">max output string size</param>
        /// <returns>random string</returns>
        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[64];

            //needs to add a couple special chars so that the string has a length of 64 which is evently divisible by 256
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_^".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        /// <summary>
        /// Will sanitize the ID values of the input object and save the results into and ouput object
        /// </summary>
        /// <typeparam name="TInput">Object with a system ID to sanitize</typeparam>
        /// <typeparam name="TOutput">Object to hold the secured key</typeparam>
        /// <param name="securableObj">Object containing a list of securable items</param>
        /// <param name="context">Session to save the mapping into</param>
        /// <param name="securedObj">Object to contain a list of secured items</param>
        /// <param name="paramName">Name of querystring parameter that this will come back from the client as</param>
        /// <param name="isOneTime">Should this item be reomved from session after it's first use</param>
        /// <returns>The mapping object that was saved into the session</returns>
        public static SecuredLookupOptions<int> Secure<TInput, TOutput>(HttpContext context, List<TInput> securableList, List<TOutput> securedList, string paramName, bool isOneTime)
            where TInput : ISecurableItem
            where TOutput : ISecuredItem, new()
        {
            SecuredLookupOptions<int> sessionValues = CreateQueryStringParameter<int>(isOneTime);

            foreach (ISecurableItem item in securableList)
            {
                //provides a random key to the actual value
                string securedKey = sessionValues.AddQueryStringOption(item.Id);

                //add to public facing list
                TOutput securedItem = new TOutput();
                securedItem.Load(item, securedKey);
                securedList.Add(securedItem);
            }

            //save values into session
            SaveQueryStringOptionsList(context, paramName, sessionValues);

            return sessionValues;
        }
    }

    /// <summary>
    /// Container to hold parameter options in the session
    /// </summary>
    /// <typeparam name="T">type of option value</typeparam>
    public class SecuredLookupOptions<T>
    {
        public bool IsOneTime { get; set; }
        public Dictionary<string, T> Options { get; set; }

        public SecuredLookupOptions(bool isOneTime)
        {
            Options = new Dictionary<string, T>();
            IsOneTime = isOneTime;
        }
    }
}
