using System;
using System.Collections.Generic;
using System.Linq;

namespace SSS.Web.Models
{
    /// <summary>
    /// list of menu options to populate a drop down list with
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
        public MenuOptionsItem<int> Default { get; private set; }

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
    /// list of menu options to populate a drop down list with
    /// </summary>
    /// <typeparam name="T">type of key for the value</typeparam>
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
        public MenuOptionsItem<T> Default { get; private set; }

        /// <summary>
        /// Wheter or not to allow a null value
        /// </summary>
        public bool AllowNull { get; set; }

        public MenuOptions()
        {
            Default = null;
        }

        /// <summary>
        /// ets the current default value by index value of the current options
        /// </summary>
        /// <param name="index">index value to select</param>
        public void SetDefaultByIndex(int index)
        {
            if (_items.Count < index)
                Default = _items[index];
            else
                throw new IndexOutOfRangeException("The index value of default did not exist in the current result set");
        }

        /// <summary>
        /// Sets the current default value by key value
        /// </summary>
        /// <param name="key">Key to search for</param>
        public void SetDefaultByKey<K>(K key)
            where K : class, T
        {
            MenuOptionsItem<T> defaultItem = _items.FirstOrDefault(x => x == key);
            if (defaultItem == null)
                throw new SSS.Utilities.Exceptions.ProgramException("The key value of default did not exist in the current result set");
        }

        /// <summary>
        /// Sets the current default value by key value
        /// </summary>
        /// <param name="key">Key to search for</param>
        public void SetDefaultByKey<K>(K? key)
            where K : struct, T
        {
            MenuOptionsItem<T> defaultItem = _items.FirstOrDefault(x => Equals(x, key));
            if (defaultItem == null)
                throw new SSS.Utilities.Exceptions.ProgramException("The key value of default did not exist in the current result set");
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
