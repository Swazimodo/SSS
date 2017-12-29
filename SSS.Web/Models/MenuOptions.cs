using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class MenuOptions
    {
        List<MenuOptionsItem<int>> _items = null;
        public List<MenuOptionsItem<int>> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<MenuOptionsItem<int>>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        /// <summary>
        /// ID of the default item
        /// </summary>
        public int? Default { get; set; }

        /// <summary>
        /// Wheter or not to allow a null value
        /// </summary>
        public bool AllowNull { get; set; }

        public MenuOptions()
        {
            Default = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MenuOptions<T>
        where T : class
    {
        List<MenuOptionsItem<T>> _items = null;
        public List<MenuOptionsItem<T>> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<MenuOptionsItem<T>>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        /// <summary>
        /// ID of the default item
        /// </summary>
        public T Default { get; set; }

        /// <summary>
        /// Wheter or not to allow a null value
        /// </summary>
        public bool AllowNull { get; set; }

        public MenuOptions()
        {
            Default = null;
        }
    }

    /////this class is to avoid having to supply generic type arguments to the static factory call
    /////https://stackoverflow.com/a/43350479/1715031
    //public static class MenuOptions<T>
    //{
    //    public static MenuOptions<T> Create<T>(T value)
    //        where T : class
    //    {
    //        return MenuOptions<T>.Create(value);
    //    }

    //    public static MenuOptions<T?> Create<T>(T? value)
    //        where T : struct
    //    {
    //        return MenuOptions<T?>.Create(value);
    //    }
    //}
}
