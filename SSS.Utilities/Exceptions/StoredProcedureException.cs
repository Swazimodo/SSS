using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSS.Utilities
{
    public class StoredProcedureException : BaseException
    {
        public int ReturnCode { get; set; }

        public StoredProcedureException(string message, int returnCode) : base(message)
        {
            ReturnCode = returnCode;
        }

        public StoredProcedureException(string message, int returnCode, Exception innerException) : base(message, innerException)
        {
            ReturnCode = returnCode;
        }
    }
}
