using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class InsertIgnoreAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class UpdateIgnoreAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DMLIgnoreAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SelectIgnoreAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IdentityAttribute : DMLIgnoreAttribute
    {
        // Presence-Only Attribute
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ColumnNameAttribute : DMLIgnoreAttribute
    {
        public readonly string columnName;
        public ColumnNameAttribute(string column)
        {
            columnName = column;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class DuplicationColumnAttribute : Attribute
    {
        public int duplicationIndex { get; private set; }

        public DuplicationColumnAttribute(int index)
        {
            duplicationIndex = index;
        }
    }
}
