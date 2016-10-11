using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
{
    public class DisparityException : Exception
    {
        public DisparityException(string message) : base(message)
        {
        }
    }

    public class InvalidTableException : Exception
    {
        public InvalidTableException(string message, Exception ex = null) : base(message, ex)
        {
        }
    }

    public class InvalidKeyException : Exception
    {
        public InvalidKeyException(string message, Exception ex = null) : base(message, ex)
        {
        }
    }

    public class InvalidModelException : Exception
    {
        public InvalidModelException(string message, Exception ex = null) : base(message, ex)
        {

        }
    }

    public class DbConnectionException : Exception
    {
        public DbConnectionException(string message, Exception ex = null) : base(message, ex)
        {

        }
    }
}
