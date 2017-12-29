using System;
using System.Collections.Generic;
using System.Text;

using SSS.Utilities.Interfaces;

namespace SSS.Web.Models
{
    public class MenuOptionsItem<T> : ISecurableItem<T>
    {
        public string Title { get; set; }
        public T Id { get; set; }

        public MenuOptionsItem() { }
        public MenuOptionsItem(string title, T id)
        {
            Title = title;
            Id = id;
        }
    }
}
