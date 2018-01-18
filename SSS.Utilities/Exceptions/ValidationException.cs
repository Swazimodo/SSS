using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities
{
    public class ValidationException : BaseException
    {
        public ValidationException(string message) : base(message)
        {

        }

        public ValidationException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
