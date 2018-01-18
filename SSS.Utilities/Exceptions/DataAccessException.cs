using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities
{
    public class DataAccessException : BaseException
    {
        public DataAccessException(string message) : base(message)
        {

        }

        public DataAccessException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
