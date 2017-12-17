using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities.Exceptions
{
    public class ConfigurationException : BaseException
    {
        public ConfigurationException(string message) : base(message)
        {

        }

        public ConfigurationException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
