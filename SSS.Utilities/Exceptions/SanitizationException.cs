using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities.Exceptions
{
    public class SanitizationException : BaseException
    {
        public SanitizationException(string message) : base(message)
        {

        }

        public SanitizationException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
