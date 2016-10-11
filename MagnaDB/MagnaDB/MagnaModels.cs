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
    public interface DataModel<out T>
    {

    }

    public abstract class TableModel<T> : DataModel<T> where T : TableModel<T>, new()
    {
        protected abstract string TableName { get; }
        protected abstract string ConnectionString { get; }
        protected abstract MagnaKey Key { get; }

        public static IEnumerable<T> ToIEnumerable()
        {
            return ToIEnumerable(string.Empty);
        }

        public static IEnumerable<T> ToIEnumerable(string extraConditions, params object[] values)
        {
            T reference = new T();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    values[i] = (values[i] as string).Replace("'", "''");
            }

            string query = GenSelect(reference.TableName) + string.Format(extraConditions, values);
            DataTable temp = TableMake(query, reference.ConnectionString, reference.TableName);

            return new List<T>();
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

        protected static IEnumerable<T> TableToEnumerable()
        {
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

    public class Nuevo : TableModel<Nuevo>
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
