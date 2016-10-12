using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public event MagnaEventHandler GetFailed;
        public event MagnaEventHandler GetSucceeded;

        public event MagnaEventHandler SelectFailed;
        public event MagnaEventHandler SelectSucceeded;

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
            query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute))), GenWhere(key));

            using (DataTable table = TableMake(query.ToString(), reference.ConnectionString, reference.TableName))
            {
                IEnumerable<T> result = Transform(table, FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute)));
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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
                reference = result.First();

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

            return null;
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
        public event MagnaEventHandler InsertSucceeded;
        public event MagnaEventHandler InsertFailed;
        public event MagnaEventHandler BeforeInsert;
        public event MagnaEventHandler UpdateSucceeded;
        public event MagnaEventHandler UpdateFailed;
        public event MagnaEventHandler BeforeUpdate;
        public event MagnaEventHandler DeleteSucceeded;
        public event MagnaEventHandler DeleteFailed;
        public event MagnaEventHandler BeforeDelete;

        public bool Insert()
        {
            BeforeInsert(this, new MagnaEventArgs(0, ConnectionString));
            
            StringBuilder query = new StringBuilder();
            query.AppendFormat(GenInsert(TableName, ToDictionary(PresenceBehavior.ExcludeAll, typeof(InsertIgnoreAttribute), typeof(DMLIgnoreAttribute), typeof(IdentityAttribute))));
            bool querySuccessful = false;

            PropertyInfo identityProperty = FilterProperties(PresenceBehavior.IncludeOnly, typeof(IdentityAttribute))?.First();

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
    }

    public class Nuevo : ViewModel<Nuevo>
    {
        protected override MagnaKey Key
        {
            get
            {
                return this.MakeKey(c => c.Id, c => c.Nombre, c => c.Fecha);
            }
        }

        public int Id { get; set; }
        public int Nombre { get; set; }
        public DateTime Fecha { get; set; }

        protected override string TableName
        {
            get
            {
                throw new NotImplementedException();
            }

        }

        protected override string ConnectionString
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
