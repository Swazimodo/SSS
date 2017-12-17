using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities.Interfaces
{
    public interface ISecurableItem
    {
        /// <summary>
        /// object contains an ID that we will secure
        /// </summary>
        int Id { get; set; }
    }
    public interface ISecurableStringItem
    {
        /// <summary>
        /// object contains an ID that we will secure
        /// </summary>
        string Id { get; set; }
    }
}
