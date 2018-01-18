using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities
{
    public class SessionException : BaseException
    {
        public SessionException(string message) : base(message)
        {

        }

        public SessionException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
