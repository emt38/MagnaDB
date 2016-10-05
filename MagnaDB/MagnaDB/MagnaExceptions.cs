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
}
