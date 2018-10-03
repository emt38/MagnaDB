using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using static MagnaDB.QueryMethods;
using static MagnaDB.SqlGenerator;

namespace MagnaDB
{
    /// <summary>
    /// This class represents an entity model that can only be retrieved from the DB, but can't be inserted,
    /// updated or deleted.
    /// </summary>
    /// <typeparam name="T">The inheriting class (used as a reciprocation)</typeparam>
    public abstract class ViewModel<T> where T : ViewModel<T>, new()
    {
        /// <summary>
        /// This event is raised when the Get method doesn't retrieve an entity from the DB or
        /// runs into an exception of some sort
        /// </summary>
        public event MagnaEventHandler GetFailed = delegate { };

        /// <summary>
        /// This event is raised when the Get method succeeds in retrieving an entity from the DB
        /// </summary>
        public event MagnaEventHandler GetSucceeded = delegate { };

        /// <summary>
        /// This event is raised when the ToList, ToIEnumerable, ToDataTable methods (and their derivations)
        /// don't retrieve an entity from the DB or run into an exception of some sort.
        /// </summary>
        public event MagnaEventHandler SelectFailed = delegate { };

        /// <summary>
        /// This event is raised when the ToList or ToIEnumerable, ToDataTable methods (and their derivations)
        /// properly retrieve an Entity from the DB.
        /// </summary>
        public event MagnaEventHandler SelectSucceeded = delegate { };

        /// <summary>
        /// The name of the DB object (can be a Table or View) where this class' entities
        /// will be retrieved from.
        /// </summary>
        protected abstract string TableName { get; }

        /// <summary>
        /// The database connection string containing the necessary information to connect to the DB.
        /// </summary>
        protected abstract string ConnectionString { get; }

        /// <summary>
        /// Represents the Primary Key for this entity's instance, composed by
        /// one or many properties of the class.
        /// </summary>
        protected abstract MagnaKey Key { get; }

        /// <summary>
        /// Used to retrieve the MagnaKey object corresponding this instance.
        /// </summary>
        /// <returns>Returns the Key Property from this instance</returns>
        public MagnaKey GetKey()
        {
            return Key;
        }

        /// <summary>
        /// Used to retrieve the DB Object's name corresponding this entity.
        /// </summary>
        /// <returns>Returns the TableName property from this object's class</returns>
        public static string GetTableName()
        {
            return new T().TableName;
        }

        /// <summary>
        /// Executes a Select Statement using this class' properties as the desired columns
        /// using this class' table and brings the results (if any) back inside a DataTable.
        /// </summary>
        /// <param name="displayableOnly">If true, only properties decorated with the DataDisplayable Attribute will be included in the select Statement</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the FROM clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a DataTable Continaing the Results obtained from the Select's execution</returns>
        public static DataTable ToDataTable(bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = TableMake(query.ToString(), reference.ConnectionString, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, reference.ConnectionString));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, reference.ConnectionString));

            return table;
        }

        /// <summary>
        /// Executes a Select Statement using this class' properties as the desired columns
        /// using this class' table and brings the results (if any) back inside a DataTable.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement Against</param>
        /// <param name="displayableOnly">If true, only properties decorated with the DataDisplayable Attribute will be included in the select Statement</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a DataTable Continaing the Results obtained from the Select's execution</returns>
        public static DataTable ToDataTable(SqlConnection connection, bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = TableMake(query.ToString(), connection, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, connection));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, connection));

            return table;
        }

        /// <summary>
        /// Executes a Select Statement using this class' properties as the desired columns
        /// using this class' table and brings the results (if any) back inside a DataTable.
        /// </summary>
        /// <param name="trans">An active SqlTransaction to execute the Select statement Against</param>
        /// <param name="displayableOnly">If true, only properties decorated with the DataDisplayable Attribute will be included in the select Statement</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a DataTable Continaing the Results obtained from the Select's execution</returns>
        public static DataTable ToDataTable(SqlTransaction trans, bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = TableMake(query.ToString(), trans, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, trans));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, trans));

            return table;
        }

        /// <summary>
        /// Executes a Select Statement using this class' properties as the desired columns
        /// using this class' table and brings the results (if any) back inside a DataTable.
        /// </summary>
        /// <param name="displayableOnly">If true, only properties decorated with the DataDisplayable Attribute will be included in the select Statement</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the FROM clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a DataTable Continaing the Results obtained from the Select's execution</returns>
        public static async Task<DataTable> ToDataTableAsync(bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = await TableMakeAsync(query.ToString(), reference.ConnectionString, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, reference.ConnectionString));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, reference.ConnectionString));

            return table;
        }

        /// <summary>
        /// Executes a Select Statement using this class' properties as the desired columns
        /// using this class' table and brings the results (if any) back inside a DataTable.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement Against</param>
        /// <param name="displayableOnly">If true, only properties decorated with the DataDisplayable Attribute will be included in the select Statement</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a DataTable Continaing the Results obtained from the Select's execution</returns>
        public static async Task<DataTable> ToDataTableAsync(SqlConnection connection, bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = await TableMakeAsync(query.ToString(), connection, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, connection));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, connection));

            return table;
        }

        /// <summary>
        /// Executes a Select Statement using this class' properties as the desired columns
        /// using this class' table and brings the results (if any) back inside a DataTable.
        /// </summary>
        /// <param name="trans">An active SqlTransaction to execute the Select statement Against</param>
        /// <param name="displayableOnly">If true, only properties decorated with the DataDisplayable Attribute will be included in the select Statement</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a DataTable Continaing the Results obtained from the Select's execution</returns>
        public static async Task<DataTable> ToDataTableAsync(SqlTransaction trans, bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = await TableMakeAsync(query.ToString(), trans, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, trans));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, trans));

            return table;
        }

        /// <summary>
        /// Transforms a DataRow array's data (which can be obtained by accessing a DataTable's Rows Property)
        /// into an IEnumerable containing entities belonging to this class.
        /// </summary>
        /// <param name="data">The DataRow array containing the information to be transformed</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="connection">An Open SqlConnection to execute statements against.</param>
        /// <returns>Returns an IEnumerable of this class containing the resulting entities of the Transformation.</returns>
        public static IEnumerable<T> TableToIEnumerable(DataRow[] data, Type[] innerModelTypes, SqlConnection connection)
        {
            return Transform(data, new T().FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
        }

        /// <summary>
        /// Transforms a DataRow array's data (which can be obtained by accessing a DataTable's Rows Property)
        /// into an IEnumerable containing entities belonging to this class.
        /// </summary>
        /// <param name="data">The DataRow array containing the information to be transformed</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="connection">An Open SqlConnection to execute statements against.</param>
        /// <returns>Returns an IEnumerable of this class containing the resulting entities of the Transformation.</returns>
        public static async Task<IEnumerable<T>> TableToIEnumerableAsync(DataRow[] data, Type[] innerModelTypes, SqlConnection connection)
        {
            return await TransformAsync(data, new T().FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
        }

        /// <summary>
        /// Transforms a DataRow array's data (which can be obtained by accessing a DataTable's Rows Property)
        /// into an IEnumerable containing entities belonging to this class.
        /// </summary>
        /// <param name="data">The DataRow array containing the information to be transformed</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="trans">An Active SqlTransaction to execute statements against.</param>
        /// <returns>Returns an IEnumerable of this class containing the resulting entities of the Transformation.</returns>
        public static IEnumerable<T> TableToIEnumerable(DataRow[] data, Type[] innerModelTypes, SqlTransaction trans)
        {
            return Transform(data, new T().FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), trans, innerModelTypes);
        }

        /// <summary>
        /// Transforms a DataRow array's data (which can be obtained by accessing a DataTable's Rows Property)
        /// into an IEnumerable containing entities belonging to this class.
        /// </summary>
        /// <param name="data">The DataRow array containing the information to be transformed</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="trans">An Active SqlTransaction to execute statements against.</param>
        /// <returns>Returns an IEnumerable of this class containing the resulting entities of the Transformation.</returns>

        public static async Task<IEnumerable<T>> TableToIEnumerableAsync(DataRow[] data, Type[] innerModelTypes, SqlTransaction trans)
        {
            return await TransformAsync(data, new T().FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), trans, innerModelTypes);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB
        /// </summary>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static IEnumerable<T> ToIEnumerable(string extraConditions = "", params object[] values)
        {
            T temp = new T();
            return ToIEnumerable(new Type[0], extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static IEnumerable<T> ToIEnumerable(Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            T temp = new T();
            using (SqlConnection gate = new SqlConnection(temp.ConnectionString))
            {
                gate.Open();
                IEnumerable<T> res = temp.ToIEnumerableInner(gate, innerModelTypes, extraConditions, values);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static IEnumerable<T> ToIEnumerable(SqlConnection connection, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return reference.ToIEnumerableInner(connection, new Type[0], extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static IEnumerable<T> ToIEnumerable(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return reference.ToIEnumerableInner(connection, innerModelTypes, extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static IEnumerable<T> ToIEnumerable(SqlTransaction trans, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return reference.ToIEnumerableInner(trans, new Type[0], extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static IEnumerable<T> ToIEnumerable(SqlTransaction trans, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return reference.ToIEnumerableInner(trans, innerModelTypes, extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        protected IEnumerable<T> ToIEnumerableInner(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;

            using (DataTable table = TableMake(query.ToString(), connection, TableName))
            {
                result = Transform(table.AsEnumerable(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
            }

            if (result.Count() > 0)
                SelectSucceeded(result, new MagnaEventArgs(result.Count(), connection));
            else
                SelectFailed(null, new MagnaEventArgs(0, connection));

            return result;
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        protected IEnumerable<T> ToIEnumerableInner(SqlTransaction trans, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;

            using (DataTable table = TableMake(query.ToString(), trans, TableName))
            {
                result = Transform(table.AsEnumerable(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), trans, innerModelTypes);
            }

            if (result.Count() > 0)
                SelectSucceeded(result, new MagnaEventArgs(result.Count(), trans));
            else
                SelectFailed(null, new MagnaEventArgs(0, trans));

            return result;
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB
        /// </summary>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<IEnumerable<T>> ToIEnumerableAsync(string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await ToIEnumerableAsync(new Type[0], extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<IEnumerable<T>> ToIEnumerableAsync(Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                await gate.OpenAsync();
                IEnumerable<T> res = await reference.ToIEnumerableAsyncInner(gate, innerModelTypes, extraConditions, values);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<IEnumerable<T>> ToIEnumerableAsync(SqlConnection connection, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await reference.ToIEnumerableAsyncInner(connection, new Type[0], extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<IEnumerable<T>> ToIEnumerableAsync(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await reference.ToIEnumerableAsyncInner(connection, innerModelTypes, extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        protected async Task<IEnumerable<T>> ToIEnumerableAsyncInner(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;

            using (DataTable table = await TableMakeAsync(query.ToString(), connection, TableName))
            {
                result = await TransformAsync(table.AsEnumerable(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
            }

            if (result.Count() > 0)
                SelectSucceeded(result, new MagnaEventArgs(result.Count(), connection));
            else
                SelectFailed(null, new MagnaEventArgs(0, connection));

            return result;
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<IEnumerable<T>> ToIEnumerableAsync(SqlTransaction trans, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await reference.ToIEnumerableAsyncInner(trans, new Type[0], extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<IEnumerable<T>> ToIEnumerableAsync(SqlTransaction trans, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await reference.ToIEnumerableAsyncInner(trans, innerModelTypes, extraConditions, values);
        }

        /// <summary>
        /// Executes a Select statement onto this class' table and retrieves its results from the DB.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        protected async Task<IEnumerable<T>> ToIEnumerableAsyncInner(SqlTransaction trans, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;

            using (DataTable table = await TableMakeAsync(query.ToString(), trans, TableName))
            {
                result = await TransformAsync(table.AsEnumerable(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), trans, innerModelTypes);
            }

            if (result.Count() > 0)
                SelectSucceeded(result, new MagnaEventArgs(result.Count(), trans));
            else
                SelectFailed(null, new MagnaEventArgs(0, trans));

            return result;
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="id">This entity's identity value.</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(long id, params Type[] innerModelTypes)
        {
            Type t = typeof(T);
            IdentityAttribute identityField;
            string idColumn = "Id";
            foreach (PropertyInfo item in t.GetProperties())
            {
                if (item.TryGetAttribute(out identityField))
                {
                    idColumn = item.Name;
                    break;
                }
            }

            T reference = new T();
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                gate.Open();
                T res = reference.GetInner(gate, new Dictionary<string, object>() { { idColumn, id } }, innerModelTypes);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="id">This entity's identity value.</param>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(SqlConnection connection, long id, params Type[] innerModelTypes)
        {
            Type t = typeof(T);
            IdentityAttribute identityField;
            string idColumn = "Id";
            foreach (PropertyInfo item in t.GetProperties())
            {
                if (item.TryGetAttribute(out identityField))
                {
                    idColumn = item.Name;
                    break;
                }
            }


            T reference = new T();
            return reference.GetInner(connection, new Dictionary<string, object>() { { idColumn, id } }, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="id">This entity's identity value.</param>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(SqlTransaction trans, long id, params Type[] innerModelTypes)
        {
            Type t = typeof(T);
            IdentityAttribute identityField;
            string idColumn = "Id";
            foreach (PropertyInfo item in t.GetProperties())
            {
                if (item.TryGetAttribute(out identityField))
                {
                    idColumn = item.Name;
                    break;
                }
            }


            T reference = new T();
            return reference.GetInner(trans, new Dictionary<string, object>() { { idColumn, id } }, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="id">This entity's identity value.</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(long id, params Type[] innerModelTypes)
        {
            Type t = typeof(T);
            IdentityAttribute identityField;
            string idColumn = "Id";
            foreach (PropertyInfo item in t.GetProperties())
            {
                if (item.TryGetAttribute(out identityField))
                {
                    idColumn = item.Name;
                    break;
                }
            }

            T reference = new T();
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                await gate.OpenAsync();
                T res = await reference.GetAsyncInner(gate, new Dictionary<string, object>() { { idColumn, id } }, innerModelTypes);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="id">This entity's identity value.</param>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(SqlConnection connection, long id, params Type[] innerModelTypes)
        {
            Type t = typeof(T);
            IdentityAttribute identityField;
            string idColumn = "Id";
            foreach (PropertyInfo item in t.GetProperties())
            {
                if (item.TryGetAttribute(out identityField))
                {
                    idColumn = item.Name;
                    break;
                }
            }


            T reference = new T();
            return await reference.GetAsyncInner(connection, new Dictionary<string, object>() { { idColumn, id } }, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="id">This entity's identity value.</param>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(SqlTransaction trans, long id, params Type[] innerModelTypes)
        {
            Type t = typeof(T);
            IdentityAttribute identityField;
            string idColumn = "Id";
            foreach (PropertyInfo item in t.GetProperties())
            {
                if (item.TryGetAttribute(out identityField))
                {
                    idColumn = item.Name;
                    break;
                }
            }


            T reference = new T();
            return await reference.GetAsyncInner(trans, new Dictionary<string, object>() { { idColumn, id } }, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                gate.Open();
                T res = reference.GetInner(gate, key, innerModelTypes);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(SqlConnection connection, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return reference.GetInner(connection, key, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(SqlTransaction trans, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return reference.GetInner(trans, key, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="connection">An Open SqlConnection to execute the Select statement against</param>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        protected T GetInner(SqlConnection connection, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), GenWhere(key));

            using (DataTable table = TableMake(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = Transform(table.AsEnumerable(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, connection));
                else
                    GetFailed(key, new MagnaEventArgs(0, connection));
            }

            return reference;
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        protected T GetInner(SqlTransaction trans, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), GenWhere(key));

            using (DataTable table = TableMake(query.ToString(), trans, TableName))
            {
                IEnumerable<T> result = Transform(table.AsEnumerable(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), trans, innerModelTypes);
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, trans));
                else
                    GetFailed(key, new MagnaEventArgs(0, trans));
            }

            return reference;
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="model">A data entity of this same class</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(T model, params Type[] innerModelTypes)
        {
            T reference = new T();
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                gate.Open();
                T res = reference.GetInner(gate, model.Key.KeyDictionary, innerModelTypes);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="model">A data entity of this same class</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(SqlConnection connection, T model, params Type[] innerModelTypes)
        {
            T reference = new T();
            return reference.GetInner(connection, model.Key.KeyDictionary, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="model">A data entity of this same class</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(SqlTransaction trans, T model, params Type[] innerModelTypes)
        {
            T reference = new T();
            return reference.GetInner(trans, model.Key.KeyDictionary, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="key">A MagnaKey object containing the key/value pairs representing an entity</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(MagnaKey key, params Type[] innerModelTypes)
        {
            T reference = new T();
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                gate.Open();
                T res = reference.GetInner(gate, key.KeyDictionary, innerModelTypes);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="key">A MagnaKey object containing the key/value pairs representing an entity</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(SqlConnection connection, MagnaKey key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return reference.GetInner(connection, key.KeyDictionary, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="key">A MagnaKey object containing the key/value pairs representing an entity</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static T Get(SqlTransaction trans, MagnaKey key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return reference.GetInner(trans, key.KeyDictionary, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                await gate.OpenAsync();
                T res = await reference.GetAsyncInner(gate, key, innerModelTypes);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(SqlConnection connection, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return await reference.GetAsyncInner(connection, key, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(SqlTransaction trans, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return await reference.GetAsyncInner(trans, key, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        protected async Task<T> GetAsyncInner(SqlConnection connection, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), GenWhere(key));

            using (DataTable table = await TableMakeAsync(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = await TransformAsync(table.AsEnumerable().ToArray(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, connection));
                else
                    GetFailed(key, new MagnaEventArgs(0, connection));
            }

            return reference;
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="key">A dictionary containing key/value pairs that will be used to retrieve the first coincidence from the Select statement</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        protected async Task<T> GetAsyncInner(SqlTransaction trans, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))), GenWhere(key));

            using (DataTable table = await TableMakeAsync(query.ToString(), trans, TableName))
            {
                IEnumerable<T> result = await TransformAsync(table.AsEnumerable().ToArray(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute)), trans, innerModelTypes);
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, trans));
                else
                    GetFailed(key, new MagnaEventArgs(0, trans));
            }

            return reference;
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="model">A data entity of this same class</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(T model, params Type[] innerModelTypes)
        {
            T reference = new T();
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                await gate.OpenAsync();
                T res = await reference.GetAsyncInner(gate, model.Key.KeyDictionary, innerModelTypes);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="model">A data entity of this same class</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(SqlConnection connection, T model, params Type[] innerModelTypes)
        {
            T reference = new T();
            return await reference.GetAsyncInner(connection, model.Key.KeyDictionary, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="model">A data entity of this same class</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(SqlTransaction trans, T model, params Type[] innerModelTypes)
        {
            T reference = new T();
            return await reference.GetAsyncInner(trans, model.Key.KeyDictionary, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="key">A MagnaKey object containing the key/value pairs representing an entity</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(MagnaKey key, params Type[] innerModelTypes)
        {
            T reference = new T();
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                await gate.OpenAsync();
                T res = await reference.GetAsyncInner(gate, key.KeyDictionary, innerModelTypes);
                gate.Close();
                return res;
            }
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="key">A MagnaKey object containing the key/value pairs representing an entity</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(SqlConnection connection, MagnaKey key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return await reference.GetAsyncInner(connection, key.KeyDictionary, innerModelTypes);
        }

        /// <summary>
        /// Retrieve an entity stored in a table row by providing the neccessary key info.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="key">A MagnaKey object containing the key/value pairs representing an entity</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>An entity of this class corresponding the key provided</returns>
        public static async Task<T> GetAsync(SqlTransaction trans, MagnaKey key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return await reference.GetAsyncInner(trans, key.KeyDictionary, innerModelTypes);
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static List<T> ToList(string extraConditions = "", params object[] values)
        {
            return ToIEnumerable(Type.EmptyTypes, extraConditions, values).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static List<T> ToList(Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return ToIEnumerable(innerModelTypes, extraConditions, values).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static List<T> ToList(SqlConnection connection, string extraConditions = "", params object[] values)
        {
            return ToIEnumerable(connection, Type.EmptyTypes, extraConditions, values).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a List of the class' type filled with the results found</returns>
        public static List<T> ToList(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return ToIEnumerable(connection, innerModelTypes, extraConditions, values).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a List of the class' type filled with the results found</returns>
        public static List<T> ToList(SqlTransaction trans, string extraConditions = "", params object[] values)
        {
            return ToIEnumerable(trans, Type.EmptyTypes, extraConditions, values).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a List of the class' type filled with the results found</returns>
        public static List<T> ToList(SqlTransaction trans, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return ToIEnumerable(trans, innerModelTypes, extraConditions, values).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<List<T>> ToListAsync(string extraConditions = "", params object[] values)
        {
            return (await ToIEnumerableAsync(Type.EmptyTypes, extraConditions, values)).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<List<T>> ToListAsync(Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return (await ToIEnumerableAsync(innerModelTypes, extraConditions, values)).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<List<T>> ToListAsync(SqlConnection connection, string extraConditions = "", params object[] values)
        {
            return (await ToIEnumerableAsync(connection, Type.EmptyTypes, extraConditions, values)).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns an IEnumerable of the class' type filled with the results found</returns>
        public static async Task<List<T>> ToListAsync(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return (await ToIEnumerableAsync(connection, innerModelTypes, extraConditions, values)).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a List of the class' type filled with the results found</returns>
        public static async Task<List<T>> ToListAsync(SqlTransaction trans, string extraConditions = "", params object[] values)
        {
            return (await ToIEnumerableAsync(trans, Type.EmptyTypes, extraConditions, values)).ToList();
        }

        /// <summary>
        /// Executes a Sql statement onto this class' table and obtains a List of entities of this class
        /// according to the specified criteria.
        /// </summary>
        /// <param name="trans">An Active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <param name="extraConditions">A format string in which SQL clauses may be added; these succeeding the From clause</param>
        /// <param name="values">Values to be formatted onto the extraConditions string</param>
        /// <returns>Returns a List of the class' type filled with the results found</returns>
        public static async Task<List<T>> ToListAsync(SqlTransaction trans, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return (await ToIEnumerableAsync(trans, innerModelTypes, extraConditions, values)).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static IEnumerable<T> LoadRelationships(IEnumerable<T> result, params Type[] innerModelTypes)
        {
            T reference = new T();
            IEnumerable<T> res;
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                gate.Open();
                res = LoadRelationships(result, gate, innerModelTypes);
                gate.Close();
            }
            return res;
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static IEnumerable<T> LoadRelationships(IEnumerable<T> result, SqlConnection connection, params Type[] innerModelTypes)
        {
            ForeignRelationAttribute fr;
            MethodInfo methodInvoker;
            Dictionary<string, Tuple<StringBuilder, Type>> tablesFiller = new Dictionary<string, Tuple<StringBuilder, Type>>();
            Dictionary<string, DataTable> propertiesTables = new Dictionary<string, DataTable>();

            string rel;
            string ownerTable = GetTableName();
            string innerTable;
            object fresult;
            IEnumerable<PropertyInfo> foreignProperties = new T().FilterProperties(PresenceBehavior.IncludeOnly, typeof(ForeignRelationAttribute)).Where(c => innerModelTypes.Any(d => d.Name == c.PropertyType.Name) || innerModelTypes.Any(d => c.PropertyType.IsGenericType && c.PropertyType?.GenericTypeArguments[0]?.Name == d.Name));

            foreach (T itera in result)
            {
                foreach (PropertyInfo p in foreignProperties)
                {
                    if (p.PropertyType.IsViewModel())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType));
                    }
                    else if (p.PropertyType.IsViewModelEnumerable())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                        continue;

                    fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                    rel = string.Format("{0}", fr.GetSelectionFormula(itera, ownerTable, innerTable));
                    tablesFiller[p.Name].Item1.AppendFormat("({0}) OR ", rel);
                }
            }

            foreach (KeyValuePair<string, Tuple<StringBuilder, Type>> item in tablesFiller)
            {
                methodInvoker = typeof(ViewModel<>).MakeGenericType(item.Value.Item2).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "ToDataTable" && m.GetParameters().Length == 4);
                rel = item.Value.Item1.ToString();
                rel = rel.Substring(0, rel.Length - 3);
                propertiesTables.Add(item.Key, (DataTable)methodInvoker.Invoke(null, new object[] { connection, false, rel, new object[0] }));
                item.Value.Item1.Clear();
            }

            if (foreignProperties.Count() > 0)
            {
                foreach (T item in result)
                {
                    foreach (PropertyInfo p in foreignProperties)
                    {
                        if (p.PropertyType.IsViewModelList())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection });
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "ToList" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType.GenericTypeArguments[0]);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModelEnumerable())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection });
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection });
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "FirstOrDefault" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static IEnumerable<T> LoadRelationships(IEnumerable<T> result, SqlTransaction trans, params Type[] innerModelTypes)
        {
            ForeignRelationAttribute fr;
            MethodInfo methodInvoker;
            Dictionary<string, Tuple<StringBuilder, Type>> tablesFiller = new Dictionary<string, Tuple<StringBuilder, Type>>();
            Dictionary<string, DataTable> propertiesTables = new Dictionary<string, DataTable>();

            string rel;
            string ownerTable = GetTableName();
            string innerTable;
            object fresult;
            IEnumerable<PropertyInfo> foreignProperties = new T().FilterProperties(PresenceBehavior.IncludeOnly, typeof(ForeignRelationAttribute)).Where(c => innerModelTypes.Any(d => d.Name == c.PropertyType.Name) || innerModelTypes.Any(d => c.PropertyType.IsGenericType && c.PropertyType?.GenericTypeArguments[0]?.Name == d.Name));

            foreach (T itera in result)
            {
                foreach (PropertyInfo p in foreignProperties)
                {
                    if (p.PropertyType.IsViewModel())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType));
                    }
                    else if (p.PropertyType.IsViewModelEnumerable())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                        continue;

                    fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                    rel = string.Format("{0}", fr.GetSelectionFormula(itera, ownerTable, innerTable));
                    tablesFiller[p.Name].Item1.AppendFormat("({0}) OR ", rel);
                }
            }

            foreach (KeyValuePair<string, Tuple<StringBuilder, Type>> item in tablesFiller)
            {
                methodInvoker = typeof(ViewModel<>).MakeGenericType(item.Value.Item2).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "ToDataTable" && m.GetParameters().Length == 4);
                rel = item.Value.Item1.ToString();
                rel = rel.Substring(0, rel.Length - 3);
                propertiesTables.Add(item.Key, (DataTable)methodInvoker.Invoke(null, new object[] { trans, false, rel, new object[0] }));
                item.Value.Item1.Clear();
            }

            if (foreignProperties.Count() > 0)
            {
                foreach (T item in result)
                {
                    foreach (PropertyInfo p in foreignProperties)
                    {
                        if (p.PropertyType.IsViewModelList())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans });
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "ToList" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType.GenericTypeArguments[0]);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModelEnumerable())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans });
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans });
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "FirstOrDefault" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public async static Task<IEnumerable<T>> LoadRelationshipsAsync(IEnumerable<T> result, params Type[] innerModelTypes)
        {
            T reference = new T();
            IEnumerable<T> res;
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                await gate.OpenAsync();
                res = await LoadRelationshipsAsync(result, gate, innerModelTypes);
                gate.Close();
            }
            return res;
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public async static Task<IEnumerable<T>> LoadRelationshipsAsync(IEnumerable<T> result, SqlConnection connection, params Type[] innerModelTypes)
        {
            ForeignRelationAttribute fr;
            MethodInfo methodInvoker;
            Dictionary<string, Tuple<StringBuilder, Type>> tablesFiller = new Dictionary<string, Tuple<StringBuilder, Type>>();
            Dictionary<string, DataTable> propertiesTables = new Dictionary<string, DataTable>();

            string rel;
            string ownerTable = GetTableName();
            string innerTable;
            object fresult;
            IEnumerable<PropertyInfo> foreignProperties = new T().FilterProperties(PresenceBehavior.IncludeOnly, typeof(ForeignRelationAttribute)).Where(c => innerModelTypes.Any(d => d.Name == c.PropertyType.Name) || innerModelTypes.Any(d => c.PropertyType.IsGenericType && c.PropertyType?.GenericTypeArguments[0]?.Name == d.Name));

            foreach (T itera in result)
            {
                foreach (PropertyInfo p in foreignProperties)
                {
                    if (p.PropertyType.IsViewModel())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType));
                    }
                    else if (p.PropertyType.IsViewModelEnumerable())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                        continue;

                    fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                    rel = string.Format("{0}", fr.GetSelectionFormula(itera, ownerTable, innerTable));
                    tablesFiller[p.Name].Item1.AppendFormat("({0}) OR ", rel);
                }
            }

            foreach (KeyValuePair<string, Tuple<StringBuilder, Type>> item in tablesFiller)
            {
                methodInvoker = typeof(ViewModel<>).MakeGenericType(item.Value.Item2).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "ToDataTableAsync" && m.GetParameters().Length == 4);
                rel = item.Value.Item1.ToString();
                rel = rel.Substring(0, rel.Length - 3);
                propertiesTables.Add(item.Key, await (Task<DataTable>)await Task.Run(() => methodInvoker.Invoke(null, new object[] { connection, false, rel, new object[0] })));
                item.Value.Item1.Clear();
            }

            if (foreignProperties.Count() > 0)
            {
                foreach (T item in result)
                {
                    foreach (PropertyInfo p in foreignProperties)
                    {
                        if (p.PropertyType.IsViewModelList())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "ToList" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType.GenericTypeArguments[0]);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModelEnumerable())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "FirstOrDefault" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public async static Task<IEnumerable<T>> LoadRelationshipsAsync(IEnumerable<T> result, SqlTransaction trans, params Type[] innerModelTypes)
        {
            ForeignRelationAttribute fr;
            MethodInfo methodInvoker;
            Dictionary<string, Tuple<StringBuilder, Type>> tablesFiller = new Dictionary<string, Tuple<StringBuilder, Type>>();
            Dictionary<string, DataTable> propertiesTables = new Dictionary<string, DataTable>();

            string rel;
            string ownerTable = GetTableName();
            string innerTable;
            object fresult;
            IEnumerable<PropertyInfo> foreignProperties = new T().FilterProperties(PresenceBehavior.IncludeOnly, typeof(ForeignRelationAttribute)).Where(c => innerModelTypes.Any(d => d.Name == c.PropertyType.Name) || innerModelTypes.Any(d => c.PropertyType.IsGenericType && c.PropertyType?.GenericTypeArguments[0]?.Name == d.Name));

            foreach (T itera in result)
            {
                foreach (PropertyInfo p in foreignProperties)
                {
                    if (p.PropertyType.IsViewModel())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType));
                    }
                    else if (p.PropertyType.IsViewModelEnumerable())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                        continue;

                    fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                    rel = string.Format("{0}", fr.GetSelectionFormula(itera, ownerTable, innerTable));
                    tablesFiller[p.Name].Item1.AppendFormat("({0}) OR ", rel);
                }
            }

            foreach (KeyValuePair<string, Tuple<StringBuilder, Type>> item in tablesFiller)
            {
                methodInvoker = typeof(ViewModel<>).MakeGenericType(item.Value.Item2).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "ToDataTableAsync" && m.GetParameters().Length == 4);
                rel = item.Value.Item1.ToString();
                rel = rel.Substring(0, rel.Length - 3);
                propertiesTables.Add(item.Key, await (Task<DataTable>)await Task.Run(() => methodInvoker.Invoke(null, new object[] { trans, false, rel, new object[0] })));
                item.Value.Item1.Clear();
            }

            if (foreignProperties.Count() > 0)
            {
                foreach (T item in result)
                {
                    foreach (PropertyInfo p in foreignProperties)
                    {
                        if (p.PropertyType.IsViewModelList())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "ToList" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType.GenericTypeArguments[0]);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModelEnumerable())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "FirstOrDefault" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entitiy to load foreign properties to</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static T LoadRelationships(T result, params Type[] innerModelTypes)
        {
            T reference = new T();
            IEnumerable<T> res;
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                gate.OpenAsync();
                res = LoadRelationships(new List<T>() { result }, gate, innerModelTypes);
                gate.Close();
            }
            return res.FirstOrDefault();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entity to load foreign properties to</param>
        /// <param name="connection">An active SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static T LoadRelationships(T result, SqlConnection connection, params Type[] innerModelTypes)
        {
            return LoadRelationships(new List<T>() { result }, connection, innerModelTypes).FirstOrDefault();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entity to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static T LoadRelationships(T result, SqlTransaction trans, params Type[] innerModelTypes)
        {
            return LoadRelationships(new List<T>() { result }, trans, innerModelTypes).FirstOrDefault();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A entitiy to load foreign properties to</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public async static Task<T> LoadRelationshipsAsync(T result, params Type[] innerModelTypes)
        {
            T reference = new T();
            IEnumerable<T> res;
            using (SqlConnection gate = new SqlConnection(reference.ConnectionString))
            {
                await gate.OpenAsync();
                res = await LoadRelationshipsAsync(new List<T>() { result }, gate, innerModelTypes);
                gate.Close();
            }
            return res.FirstOrDefault();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entity to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public async static Task<T> LoadRelationshipsAsync(T result, SqlConnection connection, params Type[] innerModelTypes)
        {
            return (await LoadRelationshipsAsync(new List<T>() { result }, connection, innerModelTypes)).FirstOrDefault();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public async static Task<T> LoadRelationshipsAsync(T result, SqlTransaction trans, params Type[] innerModelTypes)
        {
            return (await LoadRelationshipsAsync(new List<T>() { result }, trans, innerModelTypes)).FirstOrDefault();
        }

        /// <summary>
        /// Get a filtered list of this class' properties.
        /// </summary>
        /// <param name="behavior">Determines the action to do with the specified (if any) attributes</param>
        /// <param name="targetAttributes">Attributes that will be taken into account in the filtering</param>
        /// <returns>Returns a filtered IEnumerable of PropertyInfo objects</returns>
        protected IEnumerable<PropertyInfo> FilterProperties(PresenceBehavior behavior = PresenceBehavior.ExcludeAll, params Type[] targetAttributes)
        {
            List<PropertyInfo> resultProperties = new List<PropertyInfo>();
            Type type = GetType();
            MethodInfo getSetVerifier;
            IEnumerable<Attribute> iteraAttributes;

            foreach (PropertyInfo property in type.GetProperties())
            {
                getSetVerifier = property.GetSetMethod(false);
                if (getSetVerifier == null)
                    continue;

                getSetVerifier = property.GetGetMethod(false);
                if (getSetVerifier == null)
                    continue;

                iteraAttributes = property.GetCustomAttributes<Attribute>(true);
                switch (behavior)
                {
                    case PresenceBehavior.IncludeOnly:
                        if (iteraAttributes.Any(c => targetAttributes.Any(d => d == c.GetType())))
                            resultProperties.Add(property);
                        break;
                    case PresenceBehavior.ExcludeAll:
                        if (!iteraAttributes.Any(c => targetAttributes.Any(d => d == c.GetType())))
                            resultProperties.Add(property);
                        break;

                    default:
                        resultProperties.Add(property);
                        break;
                }

            }

            return resultProperties;
        }

        /// <summary>
        /// Transform a collection of rows into an IEnumerable of this class.
        /// </summary>
        /// <param name="table">A collection of rows to extract the data from</param>
        /// <param name="properties">Properties that will be mapped to and assigned from the rows data</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class' type resulting from the transformation</returns>
        protected static IEnumerable<T> Transform(IEnumerable<DataRow> table, IEnumerable<PropertyInfo> properties, SqlConnection connection, Type[] innerModelTypes)
        {
            T itera;
            Type possibleEnum;
            List<T> result = new List<T>();
            ColumnNameAttribute columnName;
            ForeignRelationAttribute fr;
            MethodInfo methodInvoker;
            Dictionary<string, Tuple<StringBuilder, Type>> tablesFiller = new Dictionary<string, Tuple<StringBuilder, Type>>();
            Dictionary<string, DataTable> propertiesTables = new Dictionary<string, DataTable>();

            string rel;
            string ownerTable = GetTableName();
            string innerTable;
            object fresult;
            IEnumerable<PropertyInfo> foreignProperties = new T().FilterProperties(PresenceBehavior.IncludeOnly, typeof(ForeignRelationAttribute)).Where(c => innerModelTypes.Any(d => d.Name == c.PropertyType.Name) || innerModelTypes.Any(d => c.PropertyType.IsGenericType && c.PropertyType?.GenericTypeArguments[0]?.Name == d.Name));

            foreach (DataRow row in table)
            {
                itera = new T();

                foreach (PropertyInfo p in properties)
                {
                    columnName = p.GetCustomAttribute<ColumnNameAttribute>();
                    object value = columnName == null ? row[p.Name] : row[columnName.Name];

                    if (value is DBNull)
                        value = null;

                    if (p.PropertyType == typeof(Nullable<>))
                    {
                        possibleEnum = Nullable.GetUnderlyingType(p.PropertyType);

                        if (possibleEnum.IsEnum)
                        {
                            if (value != null)
                            {
                                p.SetValue(itera, Enum.Parse(possibleEnum, value.ToString()));
                                continue;
                            }
                        }
                    }

                    if (p.PropertyType.IsEnum && value != null)
                    {
                        p.SetValue(itera, Enum.Parse(p.PropertyType, value.ToString()));
                        continue;
                    }

                    p.SetValue(itera, value);
                }

                foreach (PropertyInfo p in foreignProperties)
                {
                    if (p.PropertyType.IsViewModel())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType));
                    }
                    else if (p.PropertyType.IsViewModelEnumerable())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                        continue;

                    fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                    rel = string.Format("{0}", fr.GetSelectionFormula(itera, ownerTable, innerTable));
                    tablesFiller[p.Name].Item1.AppendFormat("({0}) OR ", rel);
                }

                result.Add(itera);
            }

            foreach (KeyValuePair<string, Tuple<StringBuilder, Type>> item in tablesFiller)
            {
                methodInvoker = typeof(ViewModel<>).MakeGenericType(item.Value.Item2).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "ToDataTable" && m.GetParameters().Length == 4);
                rel = item.Value.Item1.ToString();
                rel = rel.Substring(0, rel.Length - 3);
                propertiesTables.Add(item.Key, (DataTable)methodInvoker.Invoke(null, new object[] { connection, false, rel, new object[0] }));
                item.Value.Item1.Clear();
            }

            if (foreignProperties.Count() > 0)
            {
                foreach (T item in result)
                {
                    foreach (PropertyInfo p in foreignProperties)
                    {
                        if (p.PropertyType.IsViewModelList())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection });
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "ToList" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType.GenericTypeArguments[0]);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModelEnumerable())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection });
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection });
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "FirstOrDefault" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Transform a collection of rows into an IEnumerable of this class.
        /// </summary>
        /// <param name="table">A collection of rows to extract the data from</param>
        /// <param name="properties">Properties that will be mapped to and assigned from the rows data</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class' type resulting from the transformation</returns>
        protected static IEnumerable<T> Transform(IEnumerable<DataRow> table, IEnumerable<PropertyInfo> properties, SqlTransaction trans, Type[] innerModelTypes)
        {
            T itera;
            Type possibleEnum;
            List<T> result = new List<T>();
            ColumnNameAttribute columnName;
            ForeignRelationAttribute fr;
            MethodInfo methodInvoker;
            Dictionary<string, Tuple<StringBuilder, Type>> tablesFiller = new Dictionary<string, Tuple<StringBuilder, Type>>();
            Dictionary<string, DataTable> propertiesTables = new Dictionary<string, DataTable>();

            string rel;
            string ownerTable = GetTableName();
            string innerTable;
            object fresult;
            IEnumerable<PropertyInfo> foreignProperties = new T().FilterProperties(PresenceBehavior.IncludeOnly, typeof(ForeignRelationAttribute)).Where(c => innerModelTypes.Any(d => d.Name == c.PropertyType.Name) || innerModelTypes.Any(d => c.PropertyType.IsGenericType && c.PropertyType?.GenericTypeArguments[0]?.Name == d.Name));

            foreach (DataRow row in table)
            {
                itera = new T();

                foreach (PropertyInfo p in properties)
                {
                    columnName = p.GetCustomAttribute<ColumnNameAttribute>();
                    object value = columnName == null ? row[p.Name] : row[columnName.Name];

                    if (value is DBNull)
                        value = null;

                    if (p.PropertyType == typeof(Nullable<>))
                    {
                        possibleEnum = Nullable.GetUnderlyingType(p.PropertyType);

                        if (possibleEnum.IsEnum)
                        {
                            if (value != null)
                            {
                                p.SetValue(itera, Enum.Parse(possibleEnum, value.ToString()));
                                continue;
                            }
                        }
                    }

                    if (p.PropertyType.IsEnum && value != null)
                    {
                        p.SetValue(itera, Enum.Parse(p.PropertyType, value.ToString()));
                        continue;
                    }

                    p.SetValue(itera, value);
                }

                foreach (PropertyInfo p in foreignProperties)
                {
                    if (p.PropertyType.IsViewModel())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType));
                    }
                    else if (p.PropertyType.IsViewModelEnumerable())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                        continue;

                    fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                    rel = string.Format("{0}", fr.GetSelectionFormula(itera, ownerTable, innerTable));
                    tablesFiller[p.Name].Item1.AppendFormat("({0}) OR ", rel);
                }

                result.Add(itera);
            }

            foreach (KeyValuePair<string, Tuple<StringBuilder, Type>> item in tablesFiller)
            {
                methodInvoker = typeof(ViewModel<>).MakeGenericType(item.Value.Item2).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "ToDataTable" && m.GetParameters().Length == 4);
                rel = item.Value.Item1.ToString();
                rel = rel.Substring(0, rel.Length - 3);
                propertiesTables.Add(item.Key, (DataTable)methodInvoker.Invoke(null, new object[] { trans, false, rel, new object[0] }));
                item.Value.Item1.Clear();
            }

            if (foreignProperties.Count() > 0)
            {
                foreach (T item in result)
                {
                    foreach (PropertyInfo p in foreignProperties)
                    {
                        if (p.PropertyType.IsViewModelList())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans });
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "ToList" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType.GenericTypeArguments[0]);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModelEnumerable())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans });
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans });
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "FirstOrDefault" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Transform a collection of rows into an IEnumerable of this class.
        /// </summary>
        /// <param name="table">A collection of rows to extract the data from</param>
        /// <param name="properties">Properties that will be mapped to and assigned from the rows data</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class' type resulting from the transformation</returns>
        protected static async Task<IEnumerable<T>> TransformAsync(IEnumerable<DataRow> table, IEnumerable<PropertyInfo> properties, SqlConnection connection, Type[] innerModelTypes)
        {
            T itera;
            Type possibleEnum;
            List<T> result = new List<T>();
            ColumnNameAttribute columnName;
            ForeignRelationAttribute fr;
            MethodInfo methodInvoker;
            Dictionary<string, Tuple<StringBuilder, Type>> tablesFiller = new Dictionary<string, Tuple<StringBuilder, Type>>();
            Dictionary<string, DataTable> propertiesTables = new Dictionary<string, DataTable>();

            string rel;
            string ownerTable = GetTableName();
            string innerTable;
            object fresult;
            IEnumerable<PropertyInfo> foreignProperties = new T().FilterProperties(PresenceBehavior.IncludeOnly, typeof(ForeignRelationAttribute)).Where(c => innerModelTypes.Any(d => d.Name == c.PropertyType.Name) || innerModelTypes.Any(d => c.PropertyType.IsGenericType && c.PropertyType?.GenericTypeArguments[0]?.Name == d.Name));

            foreach (DataRow row in table)
            {
                itera = new T();

                foreach (PropertyInfo p in properties)
                {
                    columnName = p.GetCustomAttribute<ColumnNameAttribute>();
                    object value = columnName == null ? row[p.Name] : row[columnName.Name];

                    if (value is DBNull)
                        value = null;

                    if (p.PropertyType == typeof(Nullable<>))
                    {
                        possibleEnum = Nullable.GetUnderlyingType(p.PropertyType);

                        if (possibleEnum.IsEnum)
                        {
                            if (value != null)
                            {
                                p.SetValue(itera, Enum.Parse(possibleEnum, value.ToString()));
                                continue;
                            }
                        }
                    }

                    if (p.PropertyType.IsEnum && value != null)
                    {
                        p.SetValue(itera, Enum.Parse(p.PropertyType, value.ToString()));
                        continue;
                    }

                    p.SetValue(itera, value);
                }

                foreach (PropertyInfo p in foreignProperties)
                {
                    if (p.PropertyType.IsViewModel())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType));
                    }
                    else if (p.PropertyType.IsViewModelEnumerable())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                        continue;

                    fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                    rel = string.Format("{0}", fr.GetSelectionFormula(itera, ownerTable, innerTable));
                    tablesFiller[p.Name].Item1.AppendFormat("({0}) OR ", rel);
                }

                result.Add(itera);
            }

            foreach (KeyValuePair<string, Tuple<StringBuilder, Type>> item in tablesFiller)
            {
                methodInvoker = typeof(ViewModel<>).MakeGenericType(item.Value.Item2).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "ToDataTableAsync" && m.GetParameters().Length == 4);
                rel = item.Value.Item1.ToString();
                rel = rel.Substring(0, rel.Length - 3);
                propertiesTables.Add(item.Key, await (Task<DataTable>)await Task.Run(() => methodInvoker.Invoke(null, new object[] { connection, false, rel, new object[0] })));
                item.Value.Item1.Clear();
            }

            if (foreignProperties.Count() > 0)
            {
                foreach (T item in result)
                {
                    foreach (PropertyInfo p in foreignProperties)
                    {
                        if (p.PropertyType.IsViewModelList())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "ToList" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType.GenericTypeArguments[0]);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModelEnumerable())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, connection }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "FirstOrDefault" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Transform a collection of rows into an IEnumerable of this class.
        /// </summary>
        /// <param name="table">A collection of rows to extract the data from</param>
        /// <param name="properties">Properties that will be mapped to and assigned from the rows data</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class' type resulting from the transformation</returns>
        protected static async Task<IEnumerable<T>> TransformAsync(IEnumerable<DataRow> table, IEnumerable<PropertyInfo> properties, SqlTransaction trans, Type[] innerModelTypes)
        {
            T itera;
            Type possibleEnum;
            List<T> result = new List<T>();
            ColumnNameAttribute columnName;
            ForeignRelationAttribute fr;
            MethodInfo methodInvoker;
            Dictionary<string, Tuple<StringBuilder, Type>> tablesFiller = new Dictionary<string, Tuple<StringBuilder, Type>>();
            Dictionary<string, DataTable> propertiesTables = new Dictionary<string, DataTable>();

            string rel;
            string ownerTable = GetTableName();
            string innerTable;
            object fresult;
            IEnumerable<PropertyInfo> foreignProperties = new T().FilterProperties(PresenceBehavior.IncludeOnly, typeof(ForeignRelationAttribute)).Where(c => innerModelTypes.Any(d => d.Name == c.PropertyType.Name) || innerModelTypes.Any(d => c.PropertyType.IsGenericType && c.PropertyType?.GenericTypeArguments[0]?.Name == d.Name));

            foreach (DataRow row in table)
            {
                itera = new T();

                foreach (PropertyInfo p in properties)
                {
                    columnName = p.GetCustomAttribute<ColumnNameAttribute>();
                    object value = columnName == null ? row[p.Name] : row[columnName.Name];

                    if (value is DBNull)
                        value = null;

                    if (p.PropertyType == typeof(Nullable<>))
                    {
                        possibleEnum = Nullable.GetUnderlyingType(p.PropertyType);

                        if (possibleEnum.IsEnum)
                        {
                            if (value != null)
                            {
                                p.SetValue(itera, Enum.Parse(possibleEnum, value.ToString()));
                                continue;
                            }
                        }
                    }

                    if (p.PropertyType.IsEnum && value != null)
                    {
                        p.SetValue(itera, Enum.Parse(p.PropertyType, value.ToString()));
                        continue;
                    }

                    p.SetValue(itera, value);
                }

                foreach (PropertyInfo p in foreignProperties)
                {
                    if (p.PropertyType.IsViewModel())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType));
                    }
                    else if (p.PropertyType.IsViewModelEnumerable())
                    {
                        innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                        if (!tablesFiller.ContainsKey(p.Name))
                            tablesFiller.Add(p.Name, new Tuple<StringBuilder, Type>(new StringBuilder("WHERE "), p.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                        continue;

                    fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                    rel = string.Format("{0}", fr.GetSelectionFormula(itera, ownerTable, innerTable));
                    tablesFiller[p.Name].Item1.AppendFormat("({0}) OR ", rel);
                }

                result.Add(itera);
            }

            foreach (KeyValuePair<string, Tuple<StringBuilder, Type>> item in tablesFiller)
            {
                methodInvoker = typeof(ViewModel<>).MakeGenericType(item.Value.Item2).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "ToDataTableAsync" && m.GetParameters().Length == 4);
                rel = item.Value.Item1.ToString();
                rel = rel.Substring(0, rel.Length - 3);
                propertiesTables.Add(item.Key, await (Task<DataTable>)await Task.Run(() => methodInvoker.Invoke(null, new object[] { trans, false, rel, new object[0] })));
                item.Value.Item1.Clear();
            }

            if (foreignProperties.Count() > 0)
            {
                foreach (T item in result)
                {
                    foreach (PropertyInfo p in foreignProperties)
                    {
                        if (p.PropertyType.IsViewModelList())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "ToList" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType.GenericTypeArguments[0]);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModelEnumerable())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, trans }));
                                fresult = fresult.GetType().GetProperty("Result").GetValue(fresult);
                                methodInvoker = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.Name == "FirstOrDefault" && c.GetParameters().Length == 1).MakeGenericMethod(p.PropertyType);
                                p.SetValue(item, methodInvoker.Invoke(null, new object[] { fresult }));
                                continue;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get a filtered IEnumerable of this class' properties' name.
        /// </summary>
        /// <param name="behavior">Determines the action to do with the specified (if any) attributes</param>
        /// <param name="targetAttributes">Attributes that will be taken into account in the filtering</param>
        /// <returns>Returns a filtered string IEnumerable of this class' properties' name.</returns>
        protected IEnumerable<string> GetFields(PresenceBehavior behavior = PresenceBehavior.ExcludeAll, params Type[] targetAttributes)
        {
            Type type = GetType();
            List<string> fields = new List<string>();
            ColumnNameAttribute columnName;

            foreach (PropertyInfo property in FilterProperties(behavior, targetAttributes))
            {
                columnName = property.GetCustomAttribute<ColumnNameAttribute>();

                fields.Add(columnName == null ? property.Name : columnName.Name);
            }

            return fields;
        }

        /// <summary>
        /// Get a filtered IEnumerable of this class' properties' value.
        /// </summary>
        /// <param name="behavior">Determines the action to do with the specified (if any) attributes</param>
        /// <param name="targetAttributes">Attributes that will be taken into account in the filtering</param>
        /// <returns>Returns a filtered object IEnumerable of this class' properties' value.</returns>
        protected IEnumerable<object> GetValues(PresenceBehavior behavior = PresenceBehavior.ExcludeAll, params Type[] targetAttributes)
        {
            Type type = GetType();
            List<object> values = new List<object>();
            DateTimeTypeAttribute dateTimeSpecifier;
            DateTime possibleDate;
            object itera;

            foreach (PropertyInfo property in FilterProperties(behavior, targetAttributes))
            {
                itera = property.GetValue(this);

                if (itera != null && property.TryGetAttribute(out dateTimeSpecifier))
                {
                    possibleDate = (DateTime)itera;
                    switch (dateTimeSpecifier.DateKind)
                    {
                        case DateTimeSpecification.Date:
                            values.Add(possibleDate.ToString("yyyy-MM-dd"));
                            break;
                        default:
                        case DateTimeSpecification.DateAndTime:
                            values.Add(possibleDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            break;
                        case DateTimeSpecification.Time:
                            values.Add(possibleDate.ToString("HH:mm:ss.fff"));
                            break;
                    }
                    continue;
                }

                values.Add(itera);
            }

            return values;
        }

        /// <summary>
        /// Get a filtered Dictionary of this class' properties (names/values).
        /// </summary>
        /// <param name="behavior">Determines the action to do with the specified (if any) attributes</param>
        /// <param name="targetAttributes">Attributes that will be taken into account in the filtering</param>
        /// <returns>Returns a filtered Dictionary of this class' properties (names/values).</returns>
        public IDictionary<string, object> ToDictionary(PresenceBehavior behavior = PresenceBehavior.ExcludeAll, params Type[] targetAttributes)
        {
            Type type = GetType();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            ColumnNameAttribute columnName;
            DateTime possibleDate;
            DateTimeTypeAttribute dateTimeSpecifier;
            object itera;

            foreach (PropertyInfo property in FilterProperties(behavior, targetAttributes))
            {
                columnName = property.GetCustomAttribute<ColumnNameAttribute>();
                itera = property.GetValue(this);
                if (itera != null && property.TryGetAttribute(out dateTimeSpecifier))
                {
                    possibleDate = (DateTime)itera;
                    switch (dateTimeSpecifier.DateKind)
                    {
                        case DateTimeSpecification.Date:
                            dictionary.Add(columnName == null ? property.Name : columnName.Name, possibleDate.ToString("yyyy-MM-dd"));
                            break;
                        default:
                        case DateTimeSpecification.DateAndTime:
                            dictionary.Add(columnName == null ? property.Name : columnName.Name, possibleDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            break;
                        case DateTimeSpecification.Time:
                            dictionary.Add(columnName == null ? property.Name : columnName.Name, possibleDate.ToString("HH:mm:ss.fff"));
                            break;
                    }
                    continue;
                }

                dictionary.Add(columnName == null ? property.Name : columnName.Name, itera);
            }

            return dictionary;
        }
    }

    /// <summary>
    /// This class represents an entity model that can be inserted, updated, deleted, and selected from the DB.
    /// </summary>
    /// <typeparam name="T">The inheriting class (used as a reciprocation)</typeparam>
    public abstract class TableModel<T> : ViewModel<T> where T : TableModel<T>, new()
    {
        /// <summary>
        /// This event is raised when the Insert method successfully inserts this entity.
        /// </summary>
        public event MagnaEventHandler InsertSucceeded = delegate { };

        /// <summary>
        /// This event is raised when the Insert method runs into an exception of some sort.
        /// </summary>
        public event MagnaEventHandler InsertFailed = delegate { };

        /// <summary>
        /// This event is raised before the Insert method is executed.
        /// </summary>
        public event MagnaEventHandler BeforeInsert = delegate { };

        /// <summary>
        /// This event is raised when the Update method successfully updates this entity.
        /// </summary>
        public event MagnaEventHandler UpdateSucceeded = delegate { };

        /// <summary>
        /// This event is raised when the Update method runs into an exception of some sort.
        /// </summary>
        public event MagnaEventHandler UpdateFailed = delegate { };

        /// <summary>
        /// This event is raised before the Update method is executed.
        /// </summary>
        public event MagnaEventHandler BeforeUpdate = delegate { };

        /// <summary>
        /// This event is raised when the Delete method successfully deletes this entity.
        /// </summary>
        public event MagnaEventHandler DeleteSucceeded = delegate { };

        /// <summary>
        /// This event is raised when the Delete method runs into an exception of some sort.
        /// </summary>
        public event MagnaEventHandler DeleteFailed = delegate { };

        /// <summary>
        /// This event is raised before the Delete method is executed.
        /// </summary>
        public event MagnaEventHandler BeforeDelete = delegate { };

        /// <summary>
        /// Inserts this entity onto this class' table by mapping this instances properties to columns in the table.
        /// </summary>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public virtual bool Insert()
        {
            BeforeInsert(this, new MagnaEventArgs(0, ConnectionString));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute))));
            bool querySuccessful = false;

            PropertyInfo identityProperty = FilterProperties(PresenceBehavior.IncludeOnly, typeof(IdentityAttribute)).FirstOrDefault();

            if (identityProperty != null)
            {
                query.Append(" SELECT SCOPE_IDENTITY()");
                object result = DoScalar(query.ToString(), ConnectionString);
                if (result != null)
                {
                    querySuccessful = true;
                    identityProperty.SetValue(this, Convert.ChangeType(result, identityProperty.PropertyType));
                }
            }
            else
            {
                querySuccessful = DoQuery(query.ToString(), ConnectionString);
            }

            if (querySuccessful)
            {
                InsertSucceeded(this, new MagnaEventArgs(1, ConnectionString));
                return true;
            }
            else
            {
                InsertFailed(this, new MagnaEventArgs(0, ConnectionString));
                return false;
            }
        }

        /// <summary>
        /// Inserts this entity onto this class' table by mapping this instances properties to columns in the table.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public virtual bool Insert(SqlConnection connection)
        {
            BeforeInsert(this, new MagnaEventArgs(0, connection));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute))));
            bool querySuccessful = false;

            PropertyInfo identityProperty = FilterProperties(PresenceBehavior.IncludeOnly, typeof(IdentityAttribute))?.FirstOrDefault();

            if (identityProperty != null)
            {
                query.Append(" SELECT SCOPE_IDENTITY()");
                object result = DoScalar(query.ToString(), connection);
                if (result != null)
                {
                    querySuccessful = true;
                    identityProperty.SetValue(this, Convert.ChangeType(result, identityProperty.PropertyType));
                }
            }
            else
            {
                querySuccessful = DoQuery(query.ToString(), connection);
            }

            if (querySuccessful)
            {
                InsertSucceeded(this, new MagnaEventArgs(1, connection));
                return true;
            }
            else
            {
                InsertFailed(this, new MagnaEventArgs(0, connection));
                return false;
            }
        }

        /// <summary>
        /// Inserts this entity onto this class' table by mapping this instances properties to columns in the table.
        /// </summary>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public virtual bool Insert(SqlTransaction transaction)
        {
            BeforeInsert(this, new MagnaEventArgs(0, transaction));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute))));
            bool querySuccessful = false;

            PropertyInfo identityProperty = FilterProperties(PresenceBehavior.IncludeOnly, typeof(IdentityAttribute))?.FirstOrDefault();

            if (identityProperty != null)
            {
                query.Append(" SELECT SCOPE_IDENTITY()");
                object result = DoScalar(query.ToString(), transaction);
                if (result != null)
                {
                    querySuccessful = true;
                    identityProperty.SetValue(this, Convert.ChangeType(result, identityProperty.PropertyType));
                }
            }
            else
            {
                querySuccessful = DoQuery(query.ToString(), transaction);
            }

            if (querySuccessful)
            {
                InsertSucceeded(this, new MagnaEventArgs(1, transaction));
                return true;
            }
            else
            {
                InsertFailed(this, new MagnaEventArgs(0, transaction));
                return false;
            }
        }

        /// <summary>
        /// Inserts this entity onto this class' table by mapping this instances properties to columns in the table.
        /// </summary>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public virtual async Task<bool> InsertAsync()
        {
            BeforeInsert(this, new MagnaEventArgs(0, ConnectionString));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute))));
            bool querySuccessful = false;

            PropertyInfo identityProperty = FilterProperties(PresenceBehavior.IncludeOnly, typeof(IdentityAttribute))?.FirstOrDefault();

            if (identityProperty != null)
            {
                query.Append(" SELECT SCOPE_IDENTITY()");
                object result = await DoScalarAsync(query.ToString(), ConnectionString);
                if (result != null)
                {
                    querySuccessful = true;
                    identityProperty.SetValue(this, Convert.ChangeType(result, identityProperty.PropertyType));
                }
            }
            else
            {
                querySuccessful = await DoQueryAsync(query.ToString(), ConnectionString);
            }

            if (querySuccessful)
            {
                InsertSucceeded(this, new MagnaEventArgs(1, ConnectionString));
                return true;
            }
            else
            {
                InsertFailed(this, new MagnaEventArgs(0, ConnectionString));
                return false;
            }
        }

        /// <summary>
        /// Inserts this entity onto this class' table by mapping this instances properties to columns in the table.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public virtual async Task<bool> InsertAsync(SqlConnection connection)
        {
            BeforeInsert(this, new MagnaEventArgs(0, connection));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute))));
            bool querySuccessful = false;

            PropertyInfo identityProperty = FilterProperties(PresenceBehavior.IncludeOnly, typeof(IdentityAttribute))?.FirstOrDefault();

            if (identityProperty != null)
            {
                query.Append(" SELECT SCOPE_IDENTITY()");
                object result = await DoScalarAsync(query.ToString(), connection);
                if (result != null)
                {
                    querySuccessful = true;
                    identityProperty.SetValue(this, Convert.ChangeType(result, identityProperty.PropertyType));
                }
            }
            else
            {
                querySuccessful = await DoQueryAsync(query.ToString(), connection);
            }

            if (querySuccessful)
            {
                InsertSucceeded(this, new MagnaEventArgs(1, connection));
                return true;
            }
            else
            {
                InsertFailed(this, new MagnaEventArgs(0, connection));
                return false;
            }
        }

        /// <summary>
        /// Inserts this entity onto this class' table by mapping this instances properties to columns in the table.
        /// </summary>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public virtual async Task<bool> InsertAsync(SqlTransaction transaction)
        {
            BeforeInsert(this, new MagnaEventArgs(0, transaction));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute))));
            bool querySuccessful = false;

            PropertyInfo identityProperty = FilterProperties(PresenceBehavior.IncludeOnly, typeof(IdentityAttribute))?.FirstOrDefault();

            if (identityProperty != null)
            {
                query.Append(" SELECT SCOPE_IDENTITY()");
                object result = await DoScalarAsync(query.ToString(), transaction);
                if (result != null)
                {
                    querySuccessful = true;
                    identityProperty.SetValue(this, Convert.ChangeType(result, identityProperty.PropertyType));
                }
            }
            else
            {
                querySuccessful = await DoQueryAsync(query.ToString(), transaction);
            }

            if (querySuccessful)
            {
                InsertSucceeded(this, new MagnaEventArgs(1, transaction));
                return true;
            }
            else
            {
                InsertFailed(this, new MagnaEventArgs(0, transaction));
                return false;
            }
        }

        /// <summary>
        /// Updates this entity on this class' table using this instance's key property
        /// by mapping this instances properties to columns in the table.
        /// </summary>
        /// <returns>Returns a boolean value indicating whether the update was successful or not</returns>
        public virtual bool Update()
        {
            BeforeUpdate(this, new MagnaEventArgs(0, ConnectionString));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute));
            foreach (KeyValuePair<string, object> item in Key.KeyDictionary)
            {
                if (updateDictionary.ContainsKey(item.Key))
                    updateDictionary.Remove(item.Key);
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenUpdate(TableName, updateDictionary, 1), GenWhere(Key.KeyDictionary));

            bool result = DoQuery(query.ToString(), ConnectionString);

            if (result)
                UpdateSucceeded(this, new MagnaEventArgs(1, ConnectionString));
            else
                UpdateFailed(this, new MagnaEventArgs(0, ConnectionString));

            return result;
        }

        /// <summary>
        /// Updates this entity on this class' table using this instance's key property
        /// by mapping this instances properties to columns in the table.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Update statement against</param>
        /// <returns>Returns a boolean value indicating whether the update was successful or not</returns>
        public virtual bool Update(SqlConnection connection)
        {
            BeforeUpdate(this, new MagnaEventArgs(0, connection));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute));
            foreach (KeyValuePair<string, object> item in Key.KeyDictionary)
            {
                if (updateDictionary.ContainsKey(item.Key))
                    updateDictionary.Remove(item.Key);
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenUpdate(TableName, updateDictionary, 1), GenWhere(Key.KeyDictionary));

            bool result = DoQuery(query.ToString(), connection);

            if (result)
                UpdateSucceeded(this, new MagnaEventArgs(1, connection));
            else
                UpdateFailed(this, new MagnaEventArgs(0, connection));

            return result;
        }

        /// <summary>
        /// Updates this entity on this class' table using this instance's key property
        /// by mapping this instances properties to columns in the table.
        /// </summary>
        /// <param name="transaction">An active SqlTransaction to execute the Update statement against</param>
        /// <returns>Returns a boolean value indicating whether the update was successful or not</returns>
        public virtual bool Update(SqlTransaction transaction)
        {
            BeforeUpdate(this, new MagnaEventArgs(0, transaction));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute));
            foreach (KeyValuePair<string, object> item in Key.KeyDictionary)
            {
                if (updateDictionary.ContainsKey(item.Key))
                    updateDictionary.Remove(item.Key);
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenUpdate(TableName, updateDictionary, 1), GenWhere(Key.KeyDictionary));

            bool result = DoQuery(query.ToString(), transaction);

            if (result)
                UpdateSucceeded(this, new MagnaEventArgs(1, transaction));
            else
                UpdateFailed(this, new MagnaEventArgs(0, transaction));

            return result;
        }

        /// <summary>
        /// Updates this entity on this class' table using this instance's key property
        /// by mapping this instances properties to columns in the table.
        /// </summary>
        /// <returns>Returns a boolean value indicating whether the update was successful or not</returns>
        public virtual async Task<bool> UpdateAsync()
        {
            BeforeUpdate(this, new MagnaEventArgs(0, ConnectionString));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute));
            foreach (KeyValuePair<string, object> item in Key.KeyDictionary)
            {
                if (updateDictionary.ContainsKey(item.Key))
                    updateDictionary.Remove(item.Key);
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenUpdate(TableName, updateDictionary, 1), GenWhere(Key.KeyDictionary));

            bool result = await DoQueryAsync(query.ToString(), ConnectionString);

            if (result)
                UpdateSucceeded(this, new MagnaEventArgs(1, ConnectionString));
            else
                UpdateFailed(this, new MagnaEventArgs(0, ConnectionString));

            return result;
        }

        /// <summary>
        /// Updates this entity on this class' table using this instance's key property
        /// by mapping this instances properties to columns in the table.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Update statement against</param>
        /// <returns>Returns a boolean value indicating whether the update was successful or not</returns>
        public virtual async Task<bool> UpdateAsync(SqlConnection connection)
        {
            BeforeUpdate(this, new MagnaEventArgs(0, connection));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute));
            foreach (KeyValuePair<string, object> item in Key.KeyDictionary)
            {
                if (updateDictionary.ContainsKey(item.Key))
                    updateDictionary.Remove(item.Key);
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenUpdate(TableName, updateDictionary, 1), GenWhere(Key.KeyDictionary));

            bool result = await DoQueryAsync(query.ToString(), connection);

            if (result)
                UpdateSucceeded(this, new MagnaEventArgs(1, connection));
            else
                UpdateFailed(this, new MagnaEventArgs(0, connection));

            return result;
        }

        /// <summary>
        /// Updates this entity on this class' table using this instance's key property
        /// by mapping this instances properties to columns in the table.
        /// </summary>
        /// <param name="transaction">An active SqlTransaction to execute the Update statement against</param>
        /// <returns>Returns a boolean value indicating whether the update was successful or not</returns>
        public virtual async Task<bool> UpdateAsync(SqlTransaction transaction)
        {
            BeforeUpdate(this, new MagnaEventArgs(0, transaction));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute), typeof(ForeignRelationAttribute));
            foreach (KeyValuePair<string, object> item in Key.KeyDictionary)
            {
                if (updateDictionary.ContainsKey(item.Key))
                    updateDictionary.Remove(item.Key);
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenUpdate(TableName, updateDictionary, 1), GenWhere(Key.KeyDictionary));

            bool result = await DoQueryAsync(query.ToString(), transaction);

            if (result)
                UpdateSucceeded(this, new MagnaEventArgs(1, transaction));
            else
                UpdateFailed(this, new MagnaEventArgs(0, transaction));

            return result;
        }

        /// <summary>
        /// Deletes this entity from this class' table by using this intance's Key property.
        /// </summary>
        /// <returns>Returns a boolean value indicating whether the deletion was successful or not</returns>
        public virtual bool Delete()
        {
            StringBuilder query = new StringBuilder();
            query.Append(GenDelete(TableName, Key.KeyDictionary, 1));

            bool result = DoQuery(query.ToString(), ConnectionString);

            if (result)
                DeleteSucceeded(this, new MagnaEventArgs(1, ConnectionString));
            else
                DeleteFailed(this, new MagnaEventArgs(0, ConnectionString));

            return result;
        }

        /// <summary>
        /// Deletes this entity from this class' table by using this intance's Key property.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Delete statement against</param>
        /// <returns>Returns a boolean value indicating whether the deletion was successful or not</returns>
        public virtual bool Delete(SqlConnection connection)
        {
            StringBuilder query = new StringBuilder();
            query.Append(GenDelete(TableName, Key.KeyDictionary, 1));

            bool result = DoQuery(query.ToString(), connection);

            if (result)
                DeleteSucceeded(this, new MagnaEventArgs(1, connection));
            else
                DeleteFailed(this, new MagnaEventArgs(0, connection));

            return result;
        }

        /// <summary>
        /// Deletes this entity from this class' table by using this intance's Key property.
        /// </summary>
        /// <param name="transaction">An active SqlTransaction to execute the Delete statement against</param>
        /// <returns>Returns a boolean value indicating whether the deletion was successful or not</returns>
        public virtual bool Delete(SqlTransaction transaction)
        {
            StringBuilder query = new StringBuilder();
            query.Append(GenDelete(TableName, Key.KeyDictionary, 1));

            bool result = DoQuery(query.ToString(), transaction);

            if (result)
                DeleteSucceeded(this, new MagnaEventArgs(1, transaction));
            else
                DeleteFailed(this, new MagnaEventArgs(0, transaction));

            return result;
        }

        /// <summary>
        /// Deletes this entity from this class' table by using this intance's Key property.
        /// </summary>
        /// <returns>Returns a boolean value indicating whether the deletion was successful or not</returns>
        public virtual async Task<bool> DeleteAsync()
        {
            StringBuilder query = new StringBuilder();
            query.Append(GenDelete(TableName, Key.KeyDictionary, 1));

            bool result = await DoQueryAsync(query.ToString(), ConnectionString);

            if (result)
                DeleteSucceeded(this, new MagnaEventArgs(1, ConnectionString));
            else
                DeleteFailed(this, new MagnaEventArgs(0, ConnectionString));

            return result;
        }

        /// <summary>
        /// Deletes this entity from this class' table by using this intance's Key property.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the Delete statement against</param>
        /// <returns>Returns a boolean value indicating whether the deletion was successful or not</returns>
        public virtual async Task<bool> DeleteAsync(SqlConnection connection)
        {
            StringBuilder query = new StringBuilder();
            query.Append(GenDelete(TableName, Key.KeyDictionary, 1));

            bool result = await DoQueryAsync(query.ToString(), connection);

            if (result)
                DeleteSucceeded(this, new MagnaEventArgs(1, connection));
            else
                DeleteFailed(this, new MagnaEventArgs(0, connection));

            return result;
        }

        /// <summary>
        /// Deletes this entity from this class' table by using this intance's Key property.
        /// </summary>
        /// <param name="transaction">An active SqlTransaction to execute the Delete statement against</param>
        /// <returns>Returns a boolean value indicating whether the deletion was successful or not</returns>
        public virtual async Task<bool> DeleteAsync(SqlTransaction transaction)
        {
            StringBuilder query = new StringBuilder();
            query.Append(GenDelete(TableName, Key.KeyDictionary, 1));

            bool result = await DoQueryAsync(query.ToString(), transaction);

            if (result)
                DeleteSucceeded(this, new MagnaEventArgs(1, transaction));
            else
                DeleteFailed(this, new MagnaEventArgs(0, transaction));

            return result;
        }

        /// <summary>
        /// Arranges a Dictionary of this class properties decorated with the DuplicationColumn attribute
        /// in each of its respective indices.
        /// </summary>
        /// <returns>Returns a Dictionary object of Duplication Verification Keys</returns>
        protected Dictionary<int, Dictionary<string, object>> GetDuplicationDictionary()
        {
            Type type = GetType();
            IEnumerable<DuplicationColumnAttribute> dupliColumn;
            ColumnNameAttribute columnName;
            object itera;
            Dictionary<int, Dictionary<string, object>> keyCompositions = new Dictionary<int, Dictionary<string, object>>();
            DateTime possibleDate;
            DateTimeTypeAttribute dateTimeSpecifier;

            foreach (PropertyInfo item in type.GetProperties())
            {
                itera = item.GetValue(this);

                if (itera == null)
                    continue;

                if (item.TryGetAttribute(out dateTimeSpecifier))
                {
                    possibleDate = (DateTime)itera;
                    switch (dateTimeSpecifier.DateKind)
                    {
                        case DateTimeSpecification.Date:
                            itera = possibleDate.ToString("yyyy-MM-dd");
                            break;
                        default:
                        case DateTimeSpecification.DateAndTime:
                            itera = possibleDate.ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        case DateTimeSpecification.Time:
                            itera = possibleDate.ToString("HH:mm:ss.fff");
                            break;
                    }
                }

                dupliColumn = item.GetCustomAttributes<DuplicationColumnAttribute>();
                columnName = item.GetCustomAttribute<ColumnNameAttribute>();
                foreach (DuplicationColumnAttribute dca in dupliColumn)
                {
                    if (!keyCompositions.ContainsKey(dca.duplicationIndex))
                        keyCompositions[dca.duplicationIndex] = new Dictionary<string, object>();

                    if (keyCompositions[dca.duplicationIndex].ContainsKey(item.Name))
                        keyCompositions[dca.duplicationIndex][columnName == null ? item.Name : columnName.Name] = itera;
                    else
                        keyCompositions[dca.duplicationIndex].Add(columnName == null ? item.Name : columnName.Name, itera);
                }
            }

            return keyCompositions;
        }

        /// <summary>
        /// Verifies if the present instance is duplicated or not on this class' table by using this instace's properties
        /// decorated with the <see cref="DuplicationColumnAttribute"/>
        /// </summary>
        /// <returns>Returns true if this instance is duplicated. Otherwise, false.</returns>
        public virtual bool IsDuplicated()
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE (", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);
            temp.Append(") ");

            temp.AppendFormat("AND ({0})", GenWhereDiffered(Key.KeyDictionary, false));

            int count = Convert.ToInt32(DoScalar(temp.ToString(), ConnectionString));

            return count > 0;
        }

        /// <summary>
        /// Verifies if the present instance is duplicated or not on this class' table by using this instace's properties
        /// decorated with the <see cref="DuplicationColumnAttribute"/>.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the verification against</param>
        /// <returns>Returns true if this instance is duplicated. Otherwise, false</returns>
        public virtual bool IsDuplicated(SqlConnection connection)
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE (", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);
            temp.Append(") ");

            temp.AppendFormat("AND ({0})", GenWhereDiffered(Key.KeyDictionary, false));

            int count = Convert.ToInt32(DoScalar(temp.ToString(), connection));

            return count > 0;
        }

        /// <summary>
        /// Verifies if the present instance is duplicated or not on this class' table by using this instace's properties
        /// decorated with the <see cref="DuplicationColumnAttribute"/>.
        /// </summary>
        /// <param name="trans">An active SqlTransaction to execute the verification against</param>
        /// <returns>Returns true if this instance is duplicated. Otherwise, false</returns>
        public virtual bool IsDuplicated(SqlTransaction trans)
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE (", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);
            temp.Append(") ");

            temp.AppendFormat("AND ({0})", GenWhereDiffered(Key.KeyDictionary, false));

            int count = Convert.ToInt32(DoScalar(temp.ToString(), trans));

            return count > 0;
        }

        /// <summary>
        /// Verifies if the present instance is duplicated or not on this class' table by using this instace's properties
        /// decorated with the <see cref="DuplicationColumnAttribute"/>.
        /// </summary>
        /// <returns>Returns true if this instance is duplicated. Otherwise, false</returns>
        public virtual async Task<bool> IsDuplicatedAsync()
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE (", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);
            temp.Append(") ");

            temp.AppendFormat("AND ({0})", GenWhereDiffered(Key.KeyDictionary, false));

            int count = Convert.ToInt32(await DoScalarAsync(temp.ToString(), ConnectionString));

            return count > 0;
        }

        /// <summary>
        /// Verifies if the present instance is duplicated or not on this class' table by using this instace's properties
        /// decorated with the <see cref="DuplicationColumnAttribute"/>.
        /// </summary>
        /// <param name="connection">An open SqlConnection to execute the verification against</param>
        /// <returns>Returns true if this instance is duplicated. Otherwise, false</returns>
        public virtual async Task<bool> IsDuplicatedAsync(SqlConnection connection)
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE (", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);
            temp.Append(") ");

            temp.AppendFormat("AND ({0})", GenWhereDiffered(Key.KeyDictionary, false));

            int count = Convert.ToInt32(await DoScalarAsync(temp.ToString(), connection));

            return count > 0;
        }

        /// <summary>
        /// Verifies if the present instance is duplicated or not on this class' table by using this instace's properties
        /// decorated with the <see cref="DuplicationColumnAttribute"/>.
        /// </summary>
        /// <param name="trans">An active SqlTransaction to execute the verification against</param>
        /// <returns>Returns true if this instance is duplicated. Otherwise, false</returns>
        public virtual async Task<bool> IsDuplicatedAsync(SqlTransaction trans)
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE (", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);
            temp.Append(") ");

            temp.AppendFormat("AND ({0})", GenWhereDiffered(Key.KeyDictionary, false));

            int count = Convert.ToInt32(await DoScalarAsync(temp.ToString(), trans));

            return count > 0;
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static bool GroupInsert(IEnumerable<T> tableModels)
        {
            T reference = new T();
            return reference.GroupInsertInner(tableModels);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        protected bool GroupInsertInner(IEnumerable<T> tableModels)
        {
            if (tableModels.Count() <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            IEnumerable<string> fields = GetFields(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute));

            temp.AppendFormat("INSERT INTO {0} ({1}) VALUES ", TableName, GenFieldsEnumeration(fields));

            foreach (T item in tableModels)
            {
                temp.AppendFormat("({0}),", GenValuesEnumeration(item.GetValues(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))));
            }
            temp[temp.Length - 1] = ' ';

            return DoQuery(temp.ToString(), ConnectionString);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static bool GroupInsert(IEnumerable<T> tableModels, SqlConnection connection)
        {
            T reference = new T();
            return reference.GroupInsertInner(tableModels, connection);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        protected bool GroupInsertInner(IEnumerable<T> tableModels, SqlConnection connection)
        {
            if (tableModels.Count() <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            IEnumerable<string> fields = GetFields(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute));

            temp.AppendFormat("INSERT INTO {0} ({1}) VALUES ", TableName, GenFieldsEnumeration(fields));

            foreach (T item in tableModels)
            {
                temp.AppendFormat("({0}),", GenValuesEnumeration(item.GetValues(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))));
            }
            temp[temp.Length - 1] = ' ';

            return DoQuery(temp.ToString(), connection);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static bool GroupInsert(IEnumerable<T> tableModels, SqlTransaction transaction)
        {
            T reference = new T();
            return reference.GroupInsertInner(tableModels, transaction);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        protected bool GroupInsertInner(IEnumerable<T> tableModels, SqlTransaction transaction)
        {
            if (tableModels.Count() <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            IEnumerable<string> fields = GetFields(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute));

            temp.AppendFormat("INSERT INTO {0} ({1}) VALUES ", TableName, GenFieldsEnumeration(fields));

            foreach (T item in tableModels)
            {
                temp.AppendFormat("({0}),", GenValuesEnumeration(item.GetValues(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))));
            }
            temp[temp.Length - 1] = ' ';

            return DoQuery(temp.ToString(), transaction);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static async Task<bool> GroupInsertAsync(IEnumerable<T> tableModels)
        {
            T reference = new T();
            return await reference.GroupInsertAsyncInner(tableModels);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        protected async Task<bool> GroupInsertAsyncInner(IEnumerable<T> tableModels)
        {
            if (tableModels.Count() <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            IEnumerable<string> fields = GetFields(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute));

            temp.AppendFormat("INSERT INTO {0} ({1}) VALUES ", TableName, GenFieldsEnumeration(fields));

            foreach (T item in tableModels)
            {
                temp.AppendFormat("({0}),", GenValuesEnumeration(item.GetValues(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))));
            }
            temp[temp.Length - 1] = ' ';

            return await DoQueryAsync(temp.ToString(), ConnectionString);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static async Task<bool> GroupInsertAsync(IEnumerable<T> tableModels, SqlConnection connection)
        {
            T reference = new T();
            return await reference.GroupInsertAsyncInner(tableModels, connection);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        protected async Task<bool> GroupInsertAsyncInner(IEnumerable<T> tableModels, SqlConnection connection)
        {
            if (tableModels.Count() <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            IEnumerable<string> fields = GetFields(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute));

            temp.AppendFormat("INSERT INTO {0} ({1}) VALUES ", TableName, GenFieldsEnumeration(fields));

            foreach (T item in tableModels)
            {
                temp.AppendFormat("({0}),", GenValuesEnumeration(item.GetValues(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))));
            }
            temp[temp.Length - 1] = ' ';

            return await DoQueryAsync(temp.ToString(), connection);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static async Task<bool> GroupInsertAsync(IEnumerable<T> tableModels, SqlTransaction transaction)
        {
            T reference = new T();
            return await reference.GroupInsertAsyncInner(tableModels, transaction);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        protected async Task<bool> GroupInsertAsyncInner(IEnumerable<T> tableModels, SqlTransaction transaction)
        {
            if (tableModels.Count() <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            IEnumerable<string> fields = GetFields(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute));

            temp.AppendFormat("INSERT INTO {0} ({1}) VALUES ", TableName, GenFieldsEnumeration(fields));

            foreach (T item in tableModels)
            {
                temp.AppendFormat("({0}),", GenValuesEnumeration(item.GetValues(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(IdentityAttribute), typeof(DMLIgnoreAttribute), typeof(ForeignRelationAttribute))));
            }
            temp[temp.Length - 1] = ' ';

            return await DoQueryAsync(temp.ToString(), transaction);
        }

        /// <summary>
        /// (beta) Uses this class' properties to create this class' table in the DB.
        /// </summary>
        /// <returns>Returns a boolean value indicati</returns>
        public static bool CreateTable()
        {
            T reference = new T();
            StringBuilder temp = new StringBuilder();
            Type tipo = reference.GetType();
            IdentityAttribute identidad;
            ColumnNameAttribute columnName;

            temp.AppendFormat("CREATE TABLE {0} (", reference.TableName);

            foreach (PropertyInfo property in tipo.GetProperties())
            {
                property.TryGetAttribute(out identidad);
                columnName = property.GetCustomAttribute<ColumnNameAttribute>();

                temp.AppendFormat(" {0} {1} {2}, ", columnName?.Name ?? property.Name, property.PropertyType.ToSqlTypeNameString(), (property.PropertyType == typeof(Nullable<>) || property.PropertyType.IsClass) ? "NULL" : "NOT NULL");
            }

            temp = temp.Remove(temp.Length - 2, 2);
            temp.Append(" )");

            try
            {
                bool result = DoQuery(temp.ToString(), reference.ConnectionString);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}