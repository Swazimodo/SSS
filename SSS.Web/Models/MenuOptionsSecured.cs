using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SSS.Utilities.Exceptions;

namespace SSS.Web.Models
{
    /// <summary>
    /// Used for sensitive values where you do not want to expose the keys to an end user
    /// This also makes it really hard for them to fuzz your parameter values
    /// </summary>
    public class MenuOptionsSecured : ModelBase
    {
        string _paramName = null;
        bool _isOneTime = false;
        List<MenuOptionsSecuredItem> _items = null;

        public List<MenuOptionsSecuredItem> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<MenuOptionsSecuredItem>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        /// <summary>
        /// Secured ID of the default item
        /// </summary>
        public string Default { get; private set; }

        public MenuOptionsSecured() { }

        /// <param name="paramName">Name of the query string parameter that a value will come back as</param>
        /// <param name="isOneTime">If true session values will be disposed after first request for a value</param>
        public MenuOptionsSecured(string paramName, bool isOneTime)
        {
            if (string.IsNullOrWhiteSpace(paramName))
                throw new SSS.Utilities.Exceptions.ApplicationException("Invalid parameter name");

            _paramName = paramName;
            _isOneTime = isOneTime;
        }

        /// <summary>
        /// creates a secured options list and saves the values into session. You cannot change the options list after this point.
        /// </summary>
        /// <param name="context">current HttpContext</param>
        /// <param name="optionsList">Options list with system keys to hide</param>
        /// <param name="defaultItem">default option item</param>
        public void SecureOptionsList<T>(HttpContext context, MenuOptions<T> options, T defaultItemId = null)
            where T : class
        {
            Security.SecuredLookupOptions<T> mapping = Security.SecuredLookupOptions.Secure(context, options.Items, Items, _paramName, _isOneTime);
            Default = mapping.Options.FirstOrDefault(x => x.Value == defaultItemId).Key;

            //QueryStringOptions<T> mapping = QueryStringHelper.Secure(context, options.Items, Items, _paramName, _isOneTime);
            //Default = mapping.Options.FirstOrDefault(x => x.Value == defaultItemId).Key;
        }

        /// <summary>
        /// creates a secured options list and saves the values into session. You cannot change the options list after this point.
        /// </summary>
        /// <param name="context">current HttpContext</param>
        /// <param name="optionsList">Options list with system keys to hide</param>
        /// <param name="defaultItem">default option item</param>
        public void SecureOptionsList(HttpContext context, MenuOptions options, int? defaultItemId = null)
        {
            Security.SecuredLookupOptions<int> mapping = Security.SecuredLookupOptions.Secure(context, options.Items, Items, _paramName, _isOneTime);
            Default = mapping.Options.FirstOrDefault(x => x.Value == defaultItemId).Key;

            //QueryStringOptions<T> mapping = QueryStringHelper.Secure(context, options.Items, Items, _paramName, _isOneTime);
            //Default = mapping.Options.FirstOrDefault(x => x.Value == defaultItemId).Key;
        }
    }
}
