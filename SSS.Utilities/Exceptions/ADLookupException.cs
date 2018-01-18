using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities
{
    public class ADLookupException : BaseException
    {
        public ADLookupException(string message) : base(message)
        {

        }

        public ADLookupException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
