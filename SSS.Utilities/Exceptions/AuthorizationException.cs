﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities.Exceptions
{
    public class AuthorizationException : BaseException
    {
        public AuthorizationException(string message) : base(message)
        {

        }

        public AuthorizationException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
