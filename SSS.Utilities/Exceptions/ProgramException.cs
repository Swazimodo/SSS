using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities.Exceptions
{
    public class ProgramException : BaseException
    {
        public ProgramException(string message) : base(message)
        {

        }

        public ProgramException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
