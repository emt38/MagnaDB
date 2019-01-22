using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
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
    /// Includes static help methods for the Model Classes
    /// </summary>
    public static class ModelExtensions
    {
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
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A entitiy to load foreign properties to</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static T LoadRelationships<T>(this T result, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, innerModelTypes);
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entity to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static T LoadRelationships<T>(this T result, SqlConnection connection, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, connection, innerModelTypes);
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static T LoadRelationships<T>(this T result, SqlTransaction trans, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, trans, innerModelTypes);
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entity to load foreign properties to</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static async Task<T> LoadRelationshipsAsync<T>(this T result, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, innerModelTypes));
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entity to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static async Task<T> LoadRelationshipsAsync<T>(this T result, SqlConnection connection, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, connection, innerModelTypes));
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static async Task<T> LoadRelationshipsAsync<T>(this T result, SqlTransaction trans, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, trans, innerModelTypes));
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static List<T> LoadRelationships<T>(this List<T> result, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, innerModelTypes).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static List<T> LoadRelationships<T>(this List<T> result, SqlConnection connection, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, connection, innerModelTypes).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static List<T> LoadRelationships<T>(this List<T> result, SqlTransaction trans, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, trans, innerModelTypes).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static async Task<List<T>> LoadRelationshipsAsync<T>(this List<T> result, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, innerModelTypes)).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static async Task<List<T>> LoadRelationshipsAsync<T>(this List<T> result, SqlConnection connection, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, connection, innerModelTypes)).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static async Task<List<T>> LoadRelationshipsAsync<T>(this List<T> result, SqlTransaction trans, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, trans, innerModelTypes)).ToList();
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static bool GroupInsert<T>(this IEnumerable<T> tableModels) where T : TableModel<T>, new()
        {
            return TableModel<T>.GroupInsert(tableModels);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static bool GroupInsert<T>(this IEnumerable<T> tableModels, SqlConnection connection) where T : TableModel<T>, new()
        {
            return TableModel<T>.GroupInsert(tableModels, connection);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static bool GroupInsert<T>(this IEnumerable<T> tableModels, SqlTransaction transaction) where T : TableModel<T>, new()
        {
            return TableModel<T>.GroupInsert(tableModels, transaction);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static async Task<bool> GroupInsertAsync<T>(this IEnumerable<T> tableModels) where T : TableModel<T>, new()
        {
            return await TableModel<T>.GroupInsertAsync(tableModels);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static async Task<bool> GroupInsertAsync<T>(this IEnumerable<T> tableModels, SqlConnection connection) where T : TableModel<T>, new()
        {
            return await TableModel<T>.GroupInsertAsync(tableModels, connection);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static async Task<bool> GroupInsertAsync<T>(this IEnumerable<T> tableModels, SqlTransaction transaction) where T : TableModel<T>, new()
        {
            return await TableModel<T>.GroupInsertAsync(tableModels, transaction);
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
        /// <param name="connection">The SqlConnection used to perform the operation</param>
        public MagnaEventArgs(int nrows, SqlConnection connection)
        {
            RowsAffected = nrows;
            CurrentConnection = connection;
            ConnectionString = connection.ConnectionString;
        }

        /// <summary>
        /// Create a new instance of a MagnaEventArgs class.
        /// </summary>
        /// <param name="nrows">The number of rows affected</param>
        /// <param name="transaction">The SqlTransaction used to perform the operation</param>
        public MagnaEventArgs(int nrows, SqlTransaction transaction)
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
        public SqlConnection CurrentConnection { get; private set; }

        /// <summary>
        /// If the operation is executed against a trasaction retrieves the transaction in use
        /// </summary>
        public SqlTransaction CurrentTransaction { get; private set; }
    }
}
