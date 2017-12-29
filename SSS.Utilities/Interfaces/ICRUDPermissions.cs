using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Utilities.Interfaces
{
    /// <summary>
    /// Adds permission properties to allow a view to dynamically render a page
    /// </summary>
    public interface ICRUDPermissions
    {
        bool Create { get; set; }
        bool Read { get; set; }
        bool Update { get; set; }
        bool Delete { get; set; }
    }
}
