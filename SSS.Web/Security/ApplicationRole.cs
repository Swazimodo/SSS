using System.Collections.Generic;

namespace SSS.Web.Security
{
    /// <summary>
    /// An application security role
    /// </summary>
    public class ApplicationRole
    {
        /// <summary>
        /// Name of security role
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// List of groups that can provide this access
        /// </summary>
        public IEnumerable<string> Groups { get; set; }
    }
}
