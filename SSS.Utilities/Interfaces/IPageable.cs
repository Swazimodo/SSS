using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities
{
    public interface IPageable<T>
        where T: IPageableItem
    {
        /// <summary>
        /// one page of records
        /// </summary>
        List<T> Items { get; set; }

        /// <summary>
        /// 0 based page number
        /// </summary>
        int CurrentPageNumber { get; set; }

        /// <summary>
        /// True if there is a following page of results
        /// </summary>
        bool HasNextPage { get; set; }
    }
}
