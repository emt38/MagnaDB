using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
{
    public abstract class DataModel
    {
        protected abstract T InnerGet<T>();
        protected abstract List<T> InnerToList<T>();
    }
}
