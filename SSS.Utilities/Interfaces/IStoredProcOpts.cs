using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Utilities
{
    public interface IStoredProcOpts
    {
        /// <summary>
        /// Whether DB messages are logged
        /// </summary>
        bool LogDBMessages { get; set; }

        /// <summary>
        /// If a data table returns more than this many rows a warning is logged
        /// </summary>
        int? LogWarningMaxDBRows { get; set; }

        /// <summary>
        /// If a data table returns more than this many rows a critical exception is thrown
        /// </summary>
        int? MaxDBRowsException { get; set; }
    }
}
