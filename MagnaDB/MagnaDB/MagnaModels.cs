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

        public static string GetTableName()
        {
            return new T().TableName;
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

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = TableMake(query.ToString(), reference.ConnectionString, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, reference.ConnectionString));
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

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = TableMake(query.ToString(), connection, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, connection));
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

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = await TableMakeAsync(query.ToString(), reference.ConnectionString, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, reference.ConnectionString));
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

            if (!displayableOnly)
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));
            else
                query.AppendFormat("{0} {1}", GenSelect(reference.TableName, reference.GetFields(PresenceBehavior.IncludeOnly, typeof(DataDisplayableAttribute))), string.Format(extraConditions, values));

            DataTable table = await TableMakeAsync(query.ToString(), connection, reference.TableName);

            if (table.Rows.Count > 0)
                reference.SelectSucceeded(table, new MagnaEventArgs(table.Rows.Count, connection));
            else
                reference.SelectFailed(null, new MagnaEventArgs(0, connection));

            return table;
        }

        public static IEnumerable<T> ToIEnumerable(string extraConditions = "", params object[] values)
        {
            T temp = new T();
            return ToIEnumerable(new Type[0], extraConditions, values);
        }

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

        public static IEnumerable<T> TableToIEnumerable(DataRow[] data, Type[] innerModelTypes, SqlConnection connection)
        {
            return Transform(data, new T().FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
        }

        public static async Task<IEnumerable<T>> TableToIEnumerableAsync(DataRow[] data, Type[] innerModelTypes, SqlConnection connection)
        {
            return await TransformAsync(data, new T().FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
        }


        public static IEnumerable<T> ToIEnumerable(SqlConnection connection, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return reference.ToIEnumerableInner(connection, new Type[0], extraConditions, values);
        }

        public static IEnumerable<T> ToIEnumerable(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return reference.ToIEnumerableInner(connection, innerModelTypes, extraConditions, values);
        }

        protected IEnumerable<T> ToIEnumerableInner(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;

            using (DataTable table = TableMake(query.ToString(), connection, TableName))
            {
                result = Transform(table.AsEnumerable(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
            }

            if (result.Count() > 0)
                SelectSucceeded(result, new MagnaEventArgs(result.Count(), connection));
            else
                SelectFailed(null, new MagnaEventArgs(0, connection));

            return result;
        }

        public static async Task<IEnumerable<T>> ToIEnumerableAsync(string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await ToIEnumerableAsync(new Type[0], extraConditions, values);
        }

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

        public static async Task<IEnumerable<T>> ToIEnumerableAsync(SqlConnection connection, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await reference.ToIEnumerableAsyncInner(connection, new Type[0], extraConditions, values);
        }

        public static async Task<IEnumerable<T>> ToIEnumerableAsync(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            T reference = new T();
            return await reference.ToIEnumerableAsyncInner(connection, innerModelTypes, extraConditions, values);
        }

        protected async Task<IEnumerable<T>> ToIEnumerableAsyncInner(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute))), string.Format(extraConditions, values));

            IEnumerable<T> result;

            using (DataTable table = await TableMakeAsync(query.ToString(), connection, TableName))
            {
                result = Transform(table.AsEnumerable(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
            }

            if (result.Count() > 0)
                SelectSucceeded(result, new MagnaEventArgs(result.Count(), connection));
            else
                SelectFailed(null, new MagnaEventArgs(0, connection));

            return result;
        }

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

        public static T Get(SqlConnection gate, long id, params Type[] innerModelTypes)
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
            return reference.GetInner(gate, new Dictionary<string, object>() { { idColumn, id } }, innerModelTypes);
        }

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

        public static async Task<T> GetAsync(SqlConnection gate, long id, params Type[] innerModelTypes)
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
            return await reference.GetAsyncInner(gate, new Dictionary<string, object>() { { idColumn, id } }, innerModelTypes);
        }

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

        public static T Get(SqlConnection connection, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return reference.GetInner(connection, key, innerModelTypes);
        }

        protected T GetInner(SqlConnection connection, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute))), GenWhere(key));

            using (DataTable table = TableMake(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = Transform(table.AsEnumerable(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, connection));
                else
                    GetFailed(key, new MagnaEventArgs(0, connection));
            }

            return reference;
        }
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

        public static T Get(SqlConnection connection, T model, params Type[] innerModelTypes)
        {
            T reference = new T();
            return reference.GetInner(connection, model.Key.KeyDictionary, innerModelTypes);
        }

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

        public static T Get(SqlConnection connection, MagnaKey key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return reference.GetInner(connection, key.KeyDictionary, innerModelTypes);
        }

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

        public static async Task<T> GetAsync(SqlConnection connection, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return await reference.GetAsyncInner(connection, key, innerModelTypes);
        }

        protected async Task<T> GetAsyncInner(SqlConnection connection, IDictionary<string, object> key, params Type[] innerModelTypes)
        {
            T reference = new T();

            StringBuilder query = new StringBuilder();
            query.AppendFormat("{0} {1}", GenSelect(TableName, GetFields(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute))), GenWhere(key));

            using (DataTable table = await TableMakeAsync(query.ToString(), connection, TableName))
            {
                IEnumerable<T> result = Transform(table.AsEnumerable().ToArray(), FilterProperties(PresenceBehavior.ExcludeAll, typeof(SelectIgnoreAttribute), typeof(ForeignRelationAttribute)), connection, innerModelTypes);
                reference = result.FirstOrDefault();

                if (reference != null)
                    GetSucceeded(reference, new MagnaEventArgs(table.Rows.Count, connection));
                else
                    GetFailed(key, new MagnaEventArgs(0, connection));
            }

            return reference;
        }

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

        public static async Task<T> GetAsync(SqlConnection connection, T model, params Type[] innerModelTypes)
        {
            T reference = new T();
            return await reference.GetAsyncInner(connection, model.Key.KeyDictionary, innerModelTypes);
        }

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

        public static async Task<T> GetAsync(SqlConnection connection, MagnaKey key, params Type[] innerModelTypes)
        {
            T reference = new T();
            return await reference.GetAsyncInner(connection, key.KeyDictionary, innerModelTypes);
        }

        public static List<T> ToList(string extraConditions = "", params object[] values)
        {
            return ToIEnumerable(Type.EmptyTypes, extraConditions, values).ToList();
        }

        public static List<T> ToList(Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return ToIEnumerable(innerModelTypes, extraConditions, values).ToList();
        }

        public static List<T> ToList(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return ToIEnumerable(connection, innerModelTypes, extraConditions, values).ToList();
        }

        public static async Task<List<T>> ToListAsync(string extraConditions = "", params object[] values)
        {
            return (await ToIEnumerableAsync(Type.EmptyTypes, extraConditions, values)).ToList();
        }

        public static async Task<List<T>> ToListAsync(Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return (await ToIEnumerableAsync(innerModelTypes, extraConditions, values)).ToList();
        }

        public static async Task<List<T>> ToListAsync(SqlConnection connection, Type[] innerModelTypes, string extraConditions = "", params object[] values)
        {
            return (await ToIEnumerableAsync(connection, innerModelTypes, extraConditions, values)).ToList();
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

        protected static IEnumerable<T> Transform(IEnumerable<DataRow> table, IEnumerable<PropertyInfo> properties, SqlConnection gate, Type[] innerModelTypes)
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
                propertiesTables.Add(item.Key, (DataTable)methodInvoker.Invoke(null, new object[] { gate, false, rel, new object[0] }));
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
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, gate });
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
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, gate });
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerable").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, gate });
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

        protected static async Task<IEnumerable<T>> TransformAsync(IEnumerable<DataRow> table, IEnumerable<PropertyInfo> properties, SqlConnection gate, Type[] innerModelTypes)
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
                propertiesTables.Add(item.Key, await Task.Run(() => (DataTable)methodInvoker.Invoke(null, new object[] { gate, false, rel, new object[0] })));
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
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, gate }));
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
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, gate }));
                                p.SetValue(item, fresult);
                                continue;
                            }
                        }

                        if (p.PropertyType.IsViewModel())
                        {
                            fr = p.GetCustomAttribute<ForeignRelationAttribute>();
                            if (fr != null)
                            {
                                innerTable = (string)typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
                                rel = fr.GetSelectionFormula(item, ownerTable, innerTable, true);
                                fresult = await Task.Run(() => typeof(ViewModel<>).MakeGenericType(p.PropertyType.GenericTypeArguments[0]).GetMethods(BindingFlags.Static | BindingFlags.Public).First(c => c.GetParameters().Length == 3 && c.Name == "TableToIEnumerableAsync").Invoke(null, new object[] { propertiesTables[p.Name].Select(rel), innerModelTypes, gate }));
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
                            values.Add(possibleDate.ToString("yyyy-MM-dd hh:mm:ss"));
                            break;
                        case DateTimeSpecification.Time:
                            values.Add(possibleDate.ToString("hh:mm:ss.fff"));
                            break;
                    }
                    continue;
                }

                values.Add(itera);
            }

            return values;
        }

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
                            dictionary.Add(columnName == null ? property.Name : columnName.Name, possibleDate.ToString("yyyy-MM-dd hh:mm:ss"));
                            break;
                        case DateTimeSpecification.Time:
                            dictionary.Add(columnName == null ? property.Name : columnName.Name, possibleDate.ToString("hh:mm:ss.fff"));
                            break;
                    }
                    continue;
                }

                dictionary.Add(columnName == null ? property.Name : columnName.Name, itera);
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

        public bool Insert(SqlConnection connection)
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

        public bool Insert(SqlTransaction transaction)
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

        public async Task<bool> InsertAsync()
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

        public async Task<bool> InsertAsync(SqlConnection connection)
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

        public async Task<bool> InsertAsync(SqlTransaction transaction)
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

        public bool Update()
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

        public bool Update(SqlConnection connection)
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

        public bool Update(SqlTransaction transaction)
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

        public async Task<bool> UpdateAsync()
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

        public async Task<bool> UpdateAsync(SqlConnection connection)
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

        public async Task<bool> UpdateAsync(SqlTransaction transaction)
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
                            itera = possibleDate.ToString("yyyy-MM-dd hh:mm:ss");
                            break;
                        case DateTimeSpecification.Time:
                            itera = possibleDate.ToString("hh:mm:ss.fff");
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

        public bool IsDuplicated()
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE ", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);

            int count = Convert.ToInt32(DoScalar(temp.ToString(), ConnectionString));

            return count > 0;
        }

        public bool IsDuplicated(SqlConnection connection)
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE ", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);

            int count = Convert.ToInt32(DoScalar(temp.ToString(), connection));

            return count > 0;
        }

        public async Task<bool> IsDuplicatedAsync()
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE ", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);

            int count = Convert.ToInt32(await DoScalarAsync(temp.ToString(), ConnectionString));

            return count > 0;
        }

        public async Task<bool> IsDuplicatedAsync(SqlConnection connection)
        {
            Dictionary<int, Dictionary<string, object>> llaves = GetDuplicationDictionary();

            if (llaves.Count <= 0)
                return false;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT COUNT(*) FROM {0} WHERE ", TableName);

            foreach (KeyValuePair<int, Dictionary<string, object>> item in llaves)
            {
                temp.AppendFormat("({0}) OR", GenWhere(item.Value, false));
            }
            temp.Remove(temp.Length - 3, 3);

            int count = Convert.ToInt32(await DoScalarAsync(temp.ToString(), connection));

            return count > 0;
        }

        public static bool GroupInsert(IEnumerable<T> tableModels)
        {
            T reference = new T();
            return reference.GroupInsertInner(tableModels);
        }

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

        public static bool GroupInsert(IEnumerable<T> tableModels, SqlConnection connection)
        {
            T reference = new T();
            return reference.GroupInsertInner(tableModels, connection);
        }

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

        public static bool GroupInsert(IEnumerable<T> tableModels, SqlTransaction transaction)
        {
            T reference = new T();
            return reference.GroupInsertInner(tableModels, transaction);
        }

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

        public static async Task<bool> GroupInsertAsync(IEnumerable<T> tableModels)
        {
            T reference = new T();
            return await reference.GroupInsertAsyncInner(tableModels);
        }

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

        public static async Task<bool> GroupInsertAsync(IEnumerable<T> tableModels, SqlConnection connection)
        {
            T reference = new T();
            return await reference.GroupInsertAsyncInner(tableModels, connection);
        }

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

        public static async Task<bool> GroupInsertAsync(IEnumerable<T> tableModels, SqlTransaction transaction)
        {
            T reference = new T();
            return await reference.GroupInsertAsyncInner(tableModels, transaction);
        }

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
