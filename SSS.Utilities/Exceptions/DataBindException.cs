using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities.Exceptions
{
    public class DataBindException : BaseException
    {
        public DataBindException(string message) : base(message)
        {

        }

        public DataBindException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
