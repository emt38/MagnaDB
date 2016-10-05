using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
{
    public static class Utils
    {
        public static bool IsNumberType(this object number)
        {
            if (number == null)
                return false;

            return (number is byte || number is short || number is int || number is long ||
                    number is sbyte || number is ushort || number is uint || number is ulong ||
                    number is float || number is double || number is decimal);
        }
    }
}
