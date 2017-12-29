using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities.Interfaces
{
    public interface ISecuredItem
    {
        string Id { get; set; }

        /// <summary>
        /// Copy in all other properties besides the ID
        /// </summary>
        /// <param name="originalItem">The original item that was secured</param>
        /// <param name="key">Random key generated for the client to reference</param>
        void Load<T>(ISecurableItem<T> originalItem, string key);
    }
}
