using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MagnaDB
{
    /// <summary>
    /// A property decorated with this Attribute will be ignored when invoking the Insert() or GroupInsert()
    /// methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class InsertIgnoreAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    /// <summary>
    /// A property decorated with this Attribute will be ignored when invoking an Update() method
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class UpdateIgnoreAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    /// <summary>
    /// A property decorated with this Attribute will be ignored when invoking Insert(), Update(), Delete(), ToIEnumerable(),
    /// ToList(), ToDataTable() or Get() methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DMLIgnoreAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    /// <summary>
    /// A property decorated with this Attribute will be ignored when creating a table using the CreateTable() method
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DDLIgnoreAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    /// <summary>
    /// A property decorated with this Attribute will be ignored when invoking
    /// ToIEnumerable(), ToList(), ToDataTable() or Get() methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SelectIgnoreAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    /// <summary>
    /// This Attribute is used to specify a property marked as an identity column specification,
    /// and therefore will be updated when an Insert() Method succeeds, and can also be used as the
    /// search criteria in Get Methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IdentityAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    /// <summary>
    /// When using the ToDataTable method specifies that by default, only columns with this attribute
    /// will be selected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DataDisplayableAttribute : Attribute
    {
        // Presence-Only Attribute
    }

    /// <summary>
    /// This Attribute identifies what part of a DateTime object should be used in the database,
    /// by default, the complete date and time is used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DateTimeTypeAttribute : Attribute
    {
        /// <summary>
        /// The kind of the date
        /// </summary>
        public DateTimeSpecification DateKind { get; private set; }

        /// <summary>
        /// This Attribute identifies what part of a DateTime object should be used in the database,
        /// by default, the complete date and time is used.
        /// </summary>
        /// <param name="dateClass">The kind of the date</param>
        public DateTimeTypeAttribute(DateTimeSpecification dateClass = DateTimeSpecification.DateAndTime)
        {
            DateKind = dateClass;
        }
    }

    /// <summary>
    /// By using this attribute you can specify the name of the column this property is binded
    /// to within the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ColumnNameAttribute : Attribute
    {
        /// <summary>
        /// The name of the column the property is binded to  
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// By using this attribute you can specify the name of the column this property is binded
        /// to within the database.
        /// </summary>
        /// <param name="column"></param>
        public ColumnNameAttribute(string column)
        {
            Name = column;
        }
    }

    /// <summary>
    /// This attribute is used to specify a column belonging to a Duplication Key Dictionary,
    /// Properties with the same index will be compared as an only key within the table, by
    /// default, properties are marked to Duplication Index 0. Duplication is evaluated
    /// using the IsDuplicated() method. A property can be used within different Key combinations
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class DuplicationColumnAttribute : Attribute
    {
        /// <summary>
        /// The index of the key that the property belongs to
        /// </summary>
        public int duplicationIndex { get; private set; }

        /// <summary>
        /// Verify a possible duplication using a key combination
        /// </summary>
        /// <param name="index"></param>
        public DuplicationColumnAttribute(int index = 0)
        {
            duplicationIndex = index;
        }
    }

    /// <summary>
    /// This class is used to specify Relationships between Models and Inner Properties
    /// of ViewModel and TableModel types. Entities will be loaded when specified
    /// in parameters when using Select Functions (ToIEnumerable, Get, ToList)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ForeignRelationAttribute : Attribute
    {
        /// <summary>
        /// The string specifying the joining condition of the relationship
        /// </summary>
        public string RelationString { get; private set; }

        /// <summary>
        /// Create a relationship between the outer model class and this model property.
        /// {0} is used to refer to the Outer Model Table, {1} is referred to this Property
        /// Model Table. Columns belonging to the Outer Model must be enclosed between tags.
        /// For a more detailed example watch the SampleRelationship.cs file
        /// </summary>
        /// <param name="relStr">A string specifying the joining condition</param>
        public ForeignRelationAttribute(string relStr)
        {
            RelationString = relStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">The model that will be used for creating resulting string</param>
        /// <param name="ownerTable">The enclosing class TableName</param>
        /// <param name="innerTable">The foreign property class TableName</param>
        /// <param name="removeTableIdentifiers">This parameter specifies if a TableName should be included in the result string</param>
        /// <returns></returns>
        public string GetSelectionFormula(object model, string ownerTable, string innerTable, bool removeTableIdentifiers = false)
        {
            Type type = model.GetType();
            PropertyInfo[] propiedades = type.GetProperties();
            string relation = string.Format(RelationString, ownerTable, innerTable);
            string eval;
            object temp;
            foreach (PropertyInfo prop in propiedades)
            {
                eval = string.Format("<{0}.{1}>", ownerTable, prop.Name);
                if (relation.Contains(eval))
                {
                    temp = prop.GetValue(model);
                    relation = relation.Replace(eval, temp != null ? temp.ToString() : "NULL");
                }
            }

            return removeTableIdentifiers ? relation.Replace(string.Format("{0}.", innerTable), "") : relation;
        }
    }
}
