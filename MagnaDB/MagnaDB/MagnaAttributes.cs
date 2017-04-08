using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

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
    public sealed class IdentityAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DataDisplayableAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DateTimeTypeAttribute : Attribute
    {
        public DateTimeSpecification DateKind { get; private set; }
        public DateTimeTypeAttribute(DateTimeSpecification dateClass = DateTimeSpecification.DateAndTime)
        {
            DateKind = dateClass;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ColumnNameAttribute : Attribute
    {
        public string Name { get; private set; }
        public ColumnNameAttribute(string column)
        {
            Name = column;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class DuplicationColumnAttribute : Attribute
    {
        public int duplicationIndex { get; private set; }

        public DuplicationColumnAttribute(int index = 0)
        {
            duplicationIndex = index;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ForeignRelationAttribute : Attribute
    {
        public string RelationString { get; private set; }

        public ForeignRelationAttribute(string relStr)
        {
            RelationString = relStr;
        }

        public string GetSelectionFormula(object model, string ownerTable, string innerTable)
        {
            Type type = model.GetType();
            PropertyInfo[] propiedades = type.GetProperties();
            string relation = string.Format(RelationString, ownerTable, innerTable);
            string eval;

            foreach (PropertyInfo prop in propiedades)
            {
                eval = string.Format("{0}.{1}", ownerTable, prop.Name);
                if (relation.Contains(eval))
                {
                    relation = relation.Replace(eval, prop.GetValue(model).ToString());
                }
            }

            return relation;
        }
    }
}
