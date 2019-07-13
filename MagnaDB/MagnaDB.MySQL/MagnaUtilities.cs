using System;
using System.Collections.Generic;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB.MySQL
{
    /// <summary>
    /// The Different Kinds of Dates
    /// </summary>
    public enum DateTimeSpecification
    {
        /// <summary>
        /// Only the Day, Month and Year will be evaluated
        /// </summary>
        Date,
        /// <summary>
        /// The whole date will be evaluated
        /// </summary>
        DateAndTime,
        /// <summary>
        /// Only the, Hours, Minutes, Seconds and Milliseconds will be evaluated
        /// </summary>
        Time
    }

    /// <summary>
    /// Specifies what action to perform when using the FilterProperties Method
    /// </summary>
    public enum PresenceBehavior
    {
        /// <summary>
        /// Includes only the properties that are marked with the specified attributes
        /// </summary>
        IncludeOnly,
        /// <summary>
        /// Excludes the properties that are marked with the specified attributes
        /// </summary>
        ExcludeAll
    }

    /// <summary>
    /// A type that handles the keys for the View and Table Models
    /// </summary>
    public sealed class MagnaKey
    {
        /// <summary>
        /// The Key/Value Dictionary of the Column/Cells values
        /// </summary>
        public IDictionary<string, object> KeyDictionary { get; private set; }

        /// <summary>
        /// A type that handles the keys for the View and Table Models
        /// </summary>
        /// <param name="fieldsValues">The Keys/Values composing the key</param>
        public MagnaKey(IDictionary<string, object> fieldsValues)
        {
            KeyDictionary = fieldsValues;
        }
    }

    /// <summary>
    /// This class includes utility extensions that are used within the framework and can be also used
    /// in production code.
    /// </summary>
    public static class MagnaUtils
    {
        /// <summary>
        /// Create a new MagnaKey object by providing lambda expressions returning each property belonging to
        /// this class' key.
        /// </summary>
        /// <typeparam name="T">This class (used as reciprocation)</typeparam>
        /// <param name="value">A ViewModel or derivate instance</param>
        /// <param name="properties">The properties composing this class' key</param>
        /// <returns>Returns a new MagnaKey object</returns>
        public static MagnaKey MakeKey<T>(this T value, params Expression<Func<T, object>>[] properties) where T : ViewModel<T>, new()
        {
            Dictionary<string, object> fieldsValues = new Dictionary<string, object>();
            object iteraEvaluation;

            foreach (Expression<Func<T, object>> item in properties)
            {
                MemberExpression me = item.Body as MemberExpression;
                if (me == null)
                {
                    me = (item.Body as UnaryExpression).Operand as MemberExpression;
                }
                PropertyInfo prop = me.Member as PropertyInfo;
                iteraEvaluation = prop.GetValue(value);

                if (iteraEvaluation == null)
                    throw new InvalidKeyException("Columns/Properties belonging to a Primary Key must not be null");

                fieldsValues.Add(prop.Name, iteraEvaluation);
            }

            return new MagnaKey(fieldsValues);
        }

        /// <summary>
        /// Verifies if a given object is a numeric type or derivate.
        /// </summary>
        /// <param name="number">The object to have verified.</param>
        /// <returns>Returns true if the given object is a numeric type. Otherwise, false.</returns>
        public static bool IsNumberType(this object number)
        {
            if (number == null)
                return false;

            return (number is byte || number is short || number is int || number is long ||
                    number is sbyte || number is ushort || number is uint || number is ulong ||
                    number is float || number is double || number is decimal);
        }
        /// <summary>
        /// Tries to retrieve a CustomAttribute from an object.
        /// </summary>
        /// <typeparam name="T">The CustomAttribute's type</typeparam>
        /// <param name="property">The property to retrieve the attribute from.</param>
        /// <param name="attribute">An out variable to assign the attribute (if found) to</param>
        /// <returns>Returns a boolean value indicating whether the property had the given attribute or not.</returns>
        public static bool TryGetAttribute<T>(this PropertyInfo property, out T attribute) where T : Attribute
        {
            IEnumerable<T> attributes = property.GetCustomAttributes<T>();
            if (attributes.Count() > 0)
            {
                attribute = attributes.First();
                return true;
            }

            attribute = null;
            return false;
        }

        /// <summary>
        /// Verifies if a given type is or inherits from ViewModel
        /// </summary>
        /// <param name="t">The type to have verified</param>
        /// <returns>Returns true if the given type is or inherits from ViewModel. Otherwise, false.</returns>
        public static bool IsViewModel(this Type t)
        {
            if (!t.IsClass || t.IsGenericType)
                return false;
            else
            {
                try
                {
                    return t.IsSubclassOf(typeof(ViewModel<>).MakeGenericType(t));
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Verifies if a given type is an IEnumerable (or derivate) of a type that is or inherits from a ViewModel.
        /// </summary>
        /// <param name="t">The type to have verified</param>
        /// <returns>Returns true if the given type an IEnumerable (or derivate) of a type that is or inherits from a ViewModel. Otherwise, false.</returns>
        public static bool IsViewModelEnumerable(this Type t)
        {
            if (!(t.GenericTypeArguments.Length > 0))
                return false;
            else
            {
                Type g = t.GetGenericArguments()[0];
                if (g.IsViewModel())
                {
                    Type ver = typeof(IEnumerable<>).MakeGenericType(g);
                    return ver.IsAssignableFrom(t);
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Verifies if a given type is a List (or derivate) of a type that is or inherits from a ViewModel.
        /// </summary>
        /// <param name="t">The type to have verified</param>
        /// <returns>Returns true if the given type a List (or derivate) of a type that is or inherits from a ViewModel. Otherwise, false.</returns>
        public static bool IsViewModelList(this Type t)
        {
            if (!(t.GenericTypeArguments.Length > 0))
                return false;
            else
            {
                Type g = t.GetGenericArguments()[0];
                if (g.IsViewModel())
                {
                    Type ver = typeof(IList<>).MakeGenericType(g);
                    return ver.IsAssignableFrom(t);
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the given type is a Nullable type or not
        /// </summary>
        /// <param name="t">The type to evaluate</param>
        /// <returns>Returns true if the given type is a Nullable definition. Otherwise, false.</returns>
        public static bool IsNullable(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Gets the specified Sql Type equivalent of the given Type
        /// </summary>
        /// <param name="t">The type to evaluate</param>
        /// <returns>Returns the name of the Sql Type equivalent to the CLR type given</returns>
        public static string ToSqlTypeNameString(this Type t)
        {
            if (t.IsNullable())
            {
                return ToSqlTypeNameString(Nullable.GetUnderlyingType(t));
            }
            if (t.IsEnum)
            {
                return ToSqlTypeNameString(Enum.GetUnderlyingType(t));
            }

            if (t == typeof(byte))
                return "TINYINT";
            if (t == typeof(double))
                return "FLOAT";
            if (t == typeof(long))
                return "BIGINT";
            if (t == typeof(short))
                return "SMALLINT";
            if (t == typeof(string))
                return "NVARCHAR(256)";
            if (t == typeof(bool))
                return "BIT";
            if (t == typeof(char))
                return "NCHAR(1)";
            if (t == typeof(DateTimeOffset))
                return "DATETIMEOFFSET(7)";
            if (t == typeof(byte[]))
                return "BINARY";
            if (t == typeof(float))
                return "REAL";
            if (t == typeof(int))
                return "INT";

            return t.Name;
        }

        /// <summary>
        /// Gets the specified MySql Type equivalent of the given Type
        /// </summary>
        /// <param name="t">The type to evaluate</param>
        /// <returns>Returns the name of the Sql Type equivalent to the CLR type given</returns>
        public static string ToMySqlTypeNameString(this Type t)
        {
            if (t.IsNullable())
            {
                return ToMySqlTypeNameString(Nullable.GetUnderlyingType(t));
            }
            if (t.IsEnum)
            {
                return ToMySqlTypeNameString(Enum.GetUnderlyingType(t));
            }

            if (t == typeof(byte))
                return "tinyint unsigned";
            if (t == typeof(sbyte))
                return "tinyint";
            if (t == typeof(double))
                return "FLOAT";
            if (t == typeof(ulong))
                return "bigint unsigned";
            if (t == typeof(long))
                return "bigint";
            if (t == typeof(short))
                return "smallint";
            if (t == typeof(ushort))
                return "smallint unsigned";
            if (t == typeof(string))
                return "nvarchar(256)";
            if (t == typeof(bool))
                return "boolean";
            if (t == typeof(char))
                return "char(1)";
            if (t == typeof(DateTimeOffset))
                return "datetimeoffset";
            if (t == typeof(byte[]))
                return "binary";
            if (t == typeof(float))
                return "float";
            if (t == typeof(int))
                return "int";
            if (t == typeof(uint))
                return "int unsigned";
            if (t == typeof(decimal))
                return "decimal";

            return t.Name;
        }
    }


    /// <summary>
    /// Representa un Evento para Objetos Magnanteractive
    /// </summary>
    /// <param name="sender">La instancia que dispara el evento</param>
    /// <param name="e">Información acerca del evento</param>
    ///
    public delegate void MagnaEventHandler(object sender, MagnaEventArgs e);

    /// <summary>
    /// This class contains info about the Magna event context
    /// </summary>
    public class MagnaEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new instance of a MagnaEventArgs class.
        /// </summary>
        /// <param name="nrows">The number of rows affected</param>
        /// <param name="connectionString">The Connection String used to perform the operation</param>
        public MagnaEventArgs(int nrows, string connectionString)
        {
            RowsAffected = nrows;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Create a new instance of a MagnaEventArgs class.
        /// </summary>
        /// <param name="nrows">The number of rows affected</param>
        /// <param name="connection">The MySqlConnection used to perform the operation</param>
        public MagnaEventArgs(int nrows, MySqlConnection connection)
        {
            RowsAffected = nrows;
            CurrentConnection = connection;
            ConnectionString = connection.ConnectionString;
        }

        /// <summary>
        /// Create a new instance of a MagnaEventArgs class.
        /// </summary>
        /// <param name="nrows">The number of rows affected</param>
        /// <param name="transaction">The MySqlTransaction used to perform the operation</param>
        public MagnaEventArgs(int nrows, MySqlTransaction transaction)
        {
            RowsAffected = nrows;
            CurrentTransaction = transaction;
            CurrentConnection = transaction.Connection;
            ConnectionString = transaction.Connection.ConnectionString;
        }

        /// <summary>
        /// Represents the number of Rows affected by the executed operation
        /// </summary>
        public int RowsAffected { get; private set; }

        /// <summary>
        /// The Connection String of the Operation
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// If the operation was executed with an exposed connection retrieves the used connection
        /// </summary>
        public MySqlConnection CurrentConnection { get; private set; }

        /// <summary>
        /// If the operation is executed against a trasaction retrieves the transaction in use
        /// </summary>
        public MySqlTransaction CurrentTransaction { get; private set; }
    }
}
