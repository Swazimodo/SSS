using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public interface ISecurableItem
    //{
    //    /// <summary>
    //    /// object contains an ID that we will secure
    //    /// </summary>
    //    int Id { get; set; }
    //}

    /// <summary>
    /// allow for custom ID column types
    /// </summary>
    /// <typeparam name="T">Type of ID column that needs to be secured</typeparam>
    public interface ISecurableItem<T>
    {
        /// <summary>
        /// object contains an ID that we will secure
        /// </summary>
        T Id { get; set; }
    }
}
