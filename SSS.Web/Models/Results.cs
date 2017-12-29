using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Web.Models
{
    /// <summary>
    /// returns a list of values
    /// </summary>
    /// <typeparam name="T">Type of values to return</typeparam>
    public class Results<T>: ModelBase
    {

        List<T> _items;
        public List<T> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<T>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
