﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities
{
    public class AuthenticationException : BaseException
    {
        public AuthenticationException(string message) : base(message)
        {

        }

        public AuthenticationException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
