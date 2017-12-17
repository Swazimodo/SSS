using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities.Exceptions
{
    public class ApplicationException : BaseException
    {
        public ApplicationException(string message) : base(message)
        {

        }

        public ApplicationException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
