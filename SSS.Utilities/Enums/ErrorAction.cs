using System;
using System.Collections.Generic;
using System.Text;

namespace SSS.Utilities.Enums
{
    /// <summary>
    /// What type of standard action should a user take when an issue is encountered
    /// </summary>
    public enum ErrorAction
    {
        /// <summary>
        /// User should reload the page to referesh the content
        /// </summary>
        Reload,

        /// <summary>
        /// User should contact support with the error details
        /// </summary>
        Support,

        /// <summary>
        /// User should repeat the same request and expect differnet results
        /// </summary>
        TryAgain
    }
}
