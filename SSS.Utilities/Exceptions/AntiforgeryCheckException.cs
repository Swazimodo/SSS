using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities
{
    public class AntiforgeryCheckException : BaseException
    {
        public AntiforgeryCheckException(string message) : base(message)
        {

        }

        public AntiforgeryCheckException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
