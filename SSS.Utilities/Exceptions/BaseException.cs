using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SSS.Utilities.Exceptions
{
    public class BaseException : System.ApplicationException
    {
        const int _defaultEventId = 5000;
        public EventId EventID { get; set; }


        public BaseException(string message) : base(message)
        {
            EventID = new EventId(_defaultEventId, GetType().Name);
        }

        public BaseException(string message, EventId eventId) : base(message)
        {
            EventID = eventId;
        }

        public BaseException(string message, Exception innerException) : base(message, innerException)
        {
            EventID = new EventId(_defaultEventId, GetType().Name);
        }

        public BaseException(string message, Exception innerException, EventId eventId) : base(message, innerException)
        {
            EventID = eventId;
        }
    }
}
