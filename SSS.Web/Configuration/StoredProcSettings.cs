using SSS.Utilities.Interfaces;

namespace SSS.Web.Configuration
{
    /// <summary>
    /// Config values for SSS.Data.StoredProc
    /// </summary>
    public class StoredProcSettings : IStoredProcOpts
    {
        /// <summary>
        /// Whether DB messages are logged
        /// </summary>
        public bool LogDBMessages { get; set; }

        /// <summary>
        /// If a data table returns more than this many rows a warning is logged
        /// </summary>
        public int? LogWarningMaxDBRows { get; set; }

        /// <summary>
        /// If a data table returns more than this many rows a critical exception is thrown
        /// </summary>
        public int? MaxDBRowsException { get; set; }
    }
}
