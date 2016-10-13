﻿using System;
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
    public abstract class ViewModel<T> where T : ViewModel<T>, new()
    {
        public event MagnaEventHandler GetFailed = delegate { };
        public event MagnaEventHandler GetSucceeded = delegate { };

        public event MagnaEventHandler SelectFailed = delegate { };
        public event MagnaEventHandler SelectSucceeded = delegate { };

        protected abstract string TableName { get; }
        protected abstract string ConnectionString { get; }
        protected abstract MagnaKey Key { get; }

        public MagnaKey GetKey()
        {
            return Key;
        }

        public static DataTable ToDataTable(bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }
            
            StringBuilder query = new StringBuilder();

            if (displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = TableMake(query.ToString(), reference.ConnectionString, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(null, new MagnaEventArgs(table.Rows.Count, reference.ConnectionString));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, reference.ConnectionString));

            return table;
        }

        public static DataTable ToDataTable(SqlConnection connection, bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();

            if (displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = TableMake(query.ToString(), connection, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(null, new MagnaEventArgs(table.Rows.Count, connection));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, connection));

            return table;
        }

        public static async Task<DataTable> ToDataTableAsync(bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();

            if (displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = await TableMakeAsync(query.ToString(), reference.ConnectionString, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(null, new MagnaEventArgs(table.Rows.Count, reference.ConnectionString));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, reference.ConnectionString));

            return table;
        }

        public static async Task<DataTable> ToDataTableAsync(SqlConnection connection, bool displayableOnly = true, string extraConditions = "", params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();

            if (displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = await TableMakeAsync(query.ToString(), connection, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(null, new MagnaEventArgs(table.Rows.Count, connection));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, connection));

            return table;
        }

        public static IEnumerable<T> ToIEnumerable(string extraConditions = "", params object[] values)
        {
            T temp = new T();
            return temp.ToIEnumerableInner(extraConditions, values);
        }

        protected IEnumerable<T> ToIEnumerableInner(string extraConditions = "", params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;
            
            using (DataTable table = TableMake(query.ToString(), ConnectionString, TableName))
            {
                result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
            }

            if (result.Count() > 0)
                SelectSucceeded(null, new MagnaEventArgs(result.Count(), ConnectionString));
            else
                SelectFailed(null, new MagnaEventArgs(0, ConnectionString));

            return result;
        }
        public static IEnumerable<T> ToIEnumerable(SqlConnection connection, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return reference.ToIEnumerableInner(connection, extraConditions, values);
        }

        protected IEnumerable<T> ToIEnumerableInner(SqlConnection connection, string extraConditions = "", params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;

            using (DataTable table = TableMake(query.ToString(), connection, TableName))
            {
                result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
            }

            if (result.Count() > 0)
                SelectSucceeded(null, new MagnaEventArgs(result.Count(), connection));
            else
                SelectFailed(null, new MagnaEventArgs(0, connection));

            return result;
        }

        public static async Task<IEnumerable<T>> ToIEnumerableAsync(string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await reference.ToIEnumerableAsyncInner(extraConditions, values);
        }

        protected async Task<IEnumerable<T>> ToIEnumerableAsyncInner(string extraConditions = "", params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;

            using (DataTable table = await TableMakeAsync(query.ToString(), ConnectionString, TableName))
            {
                result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
            }

            if (result.Count() > 0)
                SelectSucceeded(null, new MagnaEventArgs(result.Count(), ConnectionString));
            else
                SelectFailed(null, new MagnaEventArgs(0, ConnectionString));

            return result;
        }
        public static async Task<IEnumerable<T>> ToIEnumerableAsync(SqlConnection connection, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await reference.ToIEnumerableAsyncInner(connection, extraConditions);
        }

        protected async Task<IEnumerable<T>> ToIEnumerableAsyncInner(SqlConnection connection, string extraConditions = "", params object[] values)
        {

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;

            using (DataTable table = await TableMakeAsync(query.ToString(), connection, TableName))
            {
                result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
            }

            if (result.Count() > 0)
                SelectSucceeded(null, new MagnaEventArgs(result.Count(), connection));
            else
                SelectFailed(null, new MagnaEventArgs(0, connection));

            return result;
        }

        public static T Get(IDictionary<string, object> key)
        {
            T reference = new T();
            return reference.GetInner(key);
        }

        protected T GetInner(IDictionary<string, object> key)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(key));

            using (DataTable table = TableMake(query.ToString(), ConnectionString, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, ConnectionString));
                else
                    GetFailed(key, new MagnaEventArgs(0, ConnectionString));
            }

            return reference;
        }

        public static T Get(SqlConnection connection, IDictionary<string, object> key)
        {
            T reference = new T();
            return reference.GetInner(connection, key);
        }

        protected T GetInner(SqlConnection connection, IDictionary<string, object> key)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(key));

            using (DataTable table = TableMake(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, connection));
                else
                    GetFailed(key, new MagnaEventArgs(0, connection));
            }
            
            return reference;
        }
        public static T Get(T model)
        {
            T reference = new T();
            return reference.GetInner(model);
        }

        protected T GetInner(T model)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(model.Key.KeyDictionary));

            using (DataTable table = TableMake(query.ToString(), ConnectionString, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, ConnectionString));
                else
                    GetFailed(model, new MagnaEventArgs(0, ConnectionString));
            }

            return reference;
        }

        public static T Get(SqlConnection connection, T model)
        {
            T reference = new T();
            return reference.GetInner(connection, model);
        }

        protected T GetInner(SqlConnection connection, T model)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(model.Key.KeyDictionary));

            using (DataTable table = TableMake(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, connection));
                else
                    GetFailed(model, new MagnaEventArgs(0, connection));
            }

            return reference;
        }

        public static T Get(MagnaKey key)
        {
            T reference = new T();
            return reference.GetInner(key);
        }

        protected T GetInner(MagnaKey key)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(key.KeyDictionary));

            using (DataTable table = TableMake(query.ToString(), ConnectionString, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, ConnectionString));
                else
                    GetFailed(key, new MagnaEventArgs(0, ConnectionString));
            }

            return reference;
        }

        public static T Get(SqlConnection connection, MagnaKey key)
        {
            T reference = new T();
            return reference.GetInner(connection, key);
        }

        protected T GetInner(SqlConnection connection, MagnaKey key)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(key.KeyDictionary));

            using (DataTable table = TableMake(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, ConnectionString));
                else
                    GetFailed(key, new MagnaEventArgs(0, ConnectionString));
            }

            return reference;
        }

        public static async Task<T> GetAsync(IDictionary<string, object> key)
        {
            T reference = new T();
            return await reference.GetAsyncInner(key);
        }

        protected async Task<T> GetAsyncInner(IDictionary<string, object> key)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(key));

            using (DataTable table = await TableMakeAsync(query.ToString(), ConnectionString, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, ConnectionString));
                else
                    GetFailed(key, new MagnaEventArgs(0, ConnectionString));
            }

            return reference;
        }

        public static async Task<T> GetAsync(SqlConnection connection, IDictionary<string, object> key)
        {
            T reference = new T();
            return await reference.GetAsyncInner(connection, key);
        }

        protected async Task<T> GetAsyncInner(SqlConnection connection, IDictionary<string, object> key)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(key));

            using (DataTable table = await TableMakeAsync(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, connection));
                else
                    GetFailed(key, new MagnaEventArgs(0, connection));
            }

            return reference;
        }

        public static async Task<T> GetAsync(T model)
        {
            T reference = new T();
            return await reference.GetAsyncInner(model);
        }

        protected async Task<T> GetAsyncInner(T model)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(model.Key.KeyDictionary));

            using (DataTable table = await TableMakeAsync(query.ToString(), ConnectionString, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, ConnectionString));
                else
                    GetFailed(model, new MagnaEventArgs(0, ConnectionString));
            }

            return reference;
        }

        public static async Task<T> GetAsync(SqlConnection connection, T model)
        {
            T reference = new T();
            return await reference.GetAsyncInner(connection, model);
        }

        protected async Task<T> GetAsyncInner(SqlConnection connection, T model)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(model.Key.KeyDictionary));

            using (DataTable table = await TableMakeAsync(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, connection));
                else
                    GetFailed(model, new MagnaEventArgs(0, connection));
            }

            return reference;
        }
        public static async Task<T> GetAsync(MagnaKey key)
        {
            T reference = new T();
            return await reference.GetAsyncInner(key);
        }

        protected async Task<T> GetAsyncInner(MagnaKey key)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(key.KeyDictionary));

            using (DataTable table = await TableMakeAsync(query.ToString(), ConnectionString, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, ConnectionString));
                else
                    GetFailed(key, new MagnaEventArgs(0, ConnectionString));
            }

            return reference;
        }

        public static async Task<T> GetAsync(SqlConnection connection, MagnaKey key)
        {
            T reference = new T();
            return await reference.GetAsyncInner(connection, key);
        }

        protected async Task<T> GetAsyncInner(SqlConnection connection, MagnaKey key)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(key.KeyDictionary));

            using (DataTable table = await TableMakeAsync(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, connection));
                else
                    GetFailed(key, new MagnaEventArgs(0, connection));
            }

            return reference;
        }

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

        protected static IEnumerable<T> Transform(DataTable table, IEnumerable<PropertyInfo> properties)
        {
            T itera;
            Type possibleEnum;
            List<T> result = new List<T>();
            ColumnNameAttribute columnName;

            foreach (DataRow row in table.Rows)
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

                result.Add(itera);
            }

            return result;
        }

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

        protected IEnumerable<object> GetValues(PresenceBehavior behavior = PresenceBehavior.ExcludeAll, params Type[] targetAttributes)
        {
            Type type = GetType();
            List<object> values = new List<object>();

            foreach (PropertyInfo property in FilterProperties(behavior, targetAttributes))
            {
                values.Add(property.GetValue(this));
            }

            return values;
        }

        public IDictionary<string, object> ToDictionary(PresenceBehavior behavior = PresenceBehavior.ExcludeAll, params Type[] targetAttributes)
        {
            Type type = GetType();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            ColumnNameAttribute columnName;

            foreach (PropertyInfo property in FilterProperties(behavior, targetAttributes))
            {
                columnName = property.GetCustomAttribute<ColumnNameAttribute>();

                dictionary.Add(columnName == null ? property.Name : columnName.Name, property.GetValue(this));
            }

            return dictionary;
        }
    }

    public abstract class TableModel<T> : ViewModel<T> where T : TableModel<T>, new()
    {
        public event MagnaEventHandler InsertSucceeded = delegate { };
        public event MagnaEventHandler InsertFailed = delegate { };
        public event MagnaEventHandler BeforeInsert = delegate { };
        public event MagnaEventHandler UpdateSucceeded = delegate { };
        public event MagnaEventHandler UpdateFailed = delegate { };
        public event MagnaEventHandler BeforeUpdate = delegate { };
        public event MagnaEventHandler DeleteSucceeded = delegate { };
        public event MagnaEventHandler DeleteFailed = delegate { };
        public event MagnaEventHandler BeforeDelete = delegate { };

        public bool Insert()
        {
            BeforeInsert(this, new MagnaEventArgs(0, ConnectionString));
            
            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute))));
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

            if(querySuccessful)
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

        public bool Insert(SqlConnection connection)
        {
            BeforeInsert(this, new MagnaEventArgs(0, connection));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute))));
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

        public bool Insert(SqlTransaction transaction)
        {
            BeforeInsert(this, new MagnaEventArgs(0, transaction));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute))));
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

        public async Task<bool> InsertAsync()
        {
            BeforeInsert(this, new MagnaEventArgs(0, ConnectionString));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute))));
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

        public async Task<bool> InsertAsync(SqlConnection connection)
        {
            BeforeInsert(this, new MagnaEventArgs(0, connection));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute))));
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

        public async Task<bool> InsertAsync(SqlTransaction transaction)
        {
            BeforeInsert(this, new MagnaEventArgs(0, transaction));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute))));
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

        public bool Update()
        {
            BeforeUpdate(this, new MagnaEventArgs(0, ConnectionString));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute));
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

        public bool Update(SqlConnection connection)
        {
            BeforeUpdate(this, new MagnaEventArgs(0, connection));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute));
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

        public bool Update(SqlTransaction transaction)
        {
            BeforeUpdate(this, new MagnaEventArgs(0, transaction));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute));
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

        public async Task<bool> UpdateAsync()
        {
            BeforeUpdate(this, new MagnaEventArgs(0, ConnectionString));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute));
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

        public async Task<bool> UpdateAsync(SqlConnection connection)
        {
            BeforeUpdate(this, new MagnaEventArgs(0, connection));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute));
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

        public async Task<bool> UpdateAsync(SqlTransaction transaction)
        {
            BeforeUpdate(this, new MagnaEventArgs(0, transaction));

            IDictionary<string, object> updateDictionary = ToDictionary(PresenceBehavior.ExcludeAll, typeof(UpdateIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute));
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

        public bool Delete()
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

        public bool Delete(SqlConnection connection)
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

        public bool Delete(SqlTransaction transaction)
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

        public async Task<bool> DeleteAsync()
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

        public async Task<bool> DeleteAsync(SqlConnection connection)
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

        public async Task<bool> DeleteAsync(SqlTransaction transaction)
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

        public bool IsDuplicated()
        {
            Type type = GetType();
            IEnumerable<DuplicationColumnAttribute> itera;
            Dictionary<int, Dictionary<string, object>> llaves = new Dictionary<int, Dictionary<string, object>>();

            foreach (PropertyInfo item in type.GetProperties())
            {
                itera = item.GetCustomAttributes<DuplicationColumnAttribute>();
                foreach (DuplicationColumnAttribute dca in itera)
                {
                    if (!llaves.ContainsKey(dca.duplicationIndex))
                        llaves[dca.duplicationIndex] = new Dictionary<string, object>();

                    if (llaves[dca.duplicationIndex].ContainsKey(item.Name))
                        llaves[dca.duplicationIndex][item.Name] = item.GetValue(this);
                    else
                        llaves[dca.duplicationIndex].Add(item.Name, item.GetValue(this));
                }
            }

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE ", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) AND", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);

            int count = Convert.ToInt32(DoScalar(temp.ToString(), ConnectionString));

            return count > 0;
        }

        public bool IsDuplicated(SqlConnection connection)
        {
            Type type = GetType();
            IEnumerable<DuplicationColumnAttribute> itera;
            Dictionary<int, Dictionary<string, object>> llaves = new Dictionary<int, Dictionary<string, object>>();

            foreach (PropertyInfo item in type.GetProperties())
            {
                itera = item.GetCustomAttributes<DuplicationColumnAttribute>();
                foreach (DuplicationColumnAttribute dca in itera)
                {
                    if (!llaves.ContainsKey(dca.duplicationIndex))
                        llaves[dca.duplicationIndex] = new Dictionary<string, object>();

                    if (llaves[dca.duplicationIndex].ContainsKey(item.Name))
                        llaves[dca.duplicationIndex][item.Name] = item.GetValue(this);
                    else
                        llaves[dca.duplicationIndex].Add(item.Name, item.GetValue(this));
                }
            }

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE ", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) AND", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);

            int count = Convert.ToInt32(DoScalar(temp.ToString(), connection));

            return count > 0;
        }

        public async Task<bool> IsDuplicatedAsync()
        {
            Type type = GetType();
            IEnumerable<DuplicationColumnAttribute> itera;
            Dictionary<int, Dictionary<string, object>> llaves = new Dictionary<int, Dictionary<string, object>>();

            foreach (PropertyInfo item in type.GetProperties())
            {
                itera = item.GetCustomAttributes<DuplicationColumnAttribute>();
                foreach (DuplicationColumnAttribute dca in itera)
                {
                    if (!llaves.ContainsKey(dca.duplicationIndex))
                        llaves[dca.duplicationIndex] = new Dictionary<string, object>();

                    if (llaves[dca.duplicationIndex].ContainsKey(item.Name))
                        llaves[dca.duplicationIndex][item.Name] = item.GetValue(this);
                    else
                        llaves[dca.duplicationIndex].Add(item.Name, item.GetValue(this));
                }
            }

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE ", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) AND", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);

            int count = Convert.ToInt32(await DoScalarAsync(temp.ToString(), ConnectionString));

            return count > 0;
        }

        public async Task<bool> IsDuplicatedAsync(SqlConnection connection)
        {
            Type type = GetType();
            IEnumerable<DuplicationColumnAttribute> itera;
            Dictionary<int, Dictionary<string, object>> llaves = new Dictionary<int, Dictionary<string, object>>();

            foreach (PropertyInfo item in type.GetProperties())
            {
                itera = item.GetCustomAttributes<DuplicationColumnAttribute>();
                foreach (DuplicationColumnAttribute dca in itera)
                {
                    if (!llaves.ContainsKey(dca.duplicationIndex))
                        llaves[dca.duplicationIndex] = new Dictionary<string, object>();

                    if (llaves[dca.duplicationIndex].ContainsKey(item.Name))
                        llaves[dca.duplicationIndex][item.Name] = item.GetValue(this);
                    else
                        llaves[dca.duplicationIndex].Add(item.Name, item.GetValue(this));
                }
            }

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE ", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) AND", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);

            int count = Convert.ToInt32(await DoScalarAsync(temp.ToString(), connection));

            return count > 0;
        }
    }
}
