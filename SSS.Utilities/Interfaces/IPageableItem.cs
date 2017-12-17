using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities.Interfaces
{
    /// <summary>
    /// interface to allow a paging control
    /// </summary>
    public interface IPageableItem
    {
        /// <summary>
        /// Use of RowNumber allows paging to work under any ordering
        /// </summary>
        int RowNumber { get; set; }
    }
}
