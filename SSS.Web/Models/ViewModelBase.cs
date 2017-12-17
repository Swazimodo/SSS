using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Web.Models
{
    /// <summary>
    /// Contains properties to save CRUD permissions to be used while rendering a view
    /// </summary>
    public class ViewModelBase : ModelBase
    {
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
