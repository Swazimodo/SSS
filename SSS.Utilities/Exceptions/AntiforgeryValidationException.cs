using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Exceptions
{
    public class AntiforgeryValidationException : BaseException
    {
        public AntiforgeryValidationException(string message) : base(message)
        {

        }

        public AntiforgeryValidationException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
