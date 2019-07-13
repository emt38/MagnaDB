using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB.MySQL
{
    /// <summary>
    /// This class includes helper methods used to create SQL statements.
    /// </summary>
    public static class SqlGenerator
    {
        /// <summary>
        /// Create SQL Select statement.
        /// </summary>
        /// <param name="tableName">The table to select data from</param>
        /// <param name="fields">The desired columns to retrieve</param>
        /// <returns>Returns a string containing the resulting Select statement.</returns>
        public static string GenSelect(string tableName, params string[] fields)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidTableException("You must specify a Table Name to perform this operation");

            if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT {0} FROM {1}", GenFieldsEnumeration(fields), tableName);

            return temp.ToString();
        }

        /// <summary>
        /// Create SQL Select statement.
        /// </summary>
        /// <param name="tableName">The table to select data from</param>
        /// <param name="fields">The desired columns to retrieve</param>
        /// <returns>Returns a string containing the resulting Select statement.</returns>
        public static string GenSelect(string tableName, IEnumerable<string> fields)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidTableException("You must specify a Table Name to perform this operation");

            if (fields.Count() <= 0)
                return string.Empty;
            
            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("SELECT {0} FROM {1}", GenFieldsEnumeration(fields), tableName);
            
            return temp.ToString();
        }

        /// <summary>
        /// Create a SQL Insert statement.
        /// </summary>
        /// <param name="tableName">The table to insert data into</param>
        /// <param name="fieldsValues">A collection of key/value pairs that will be included in the Insert statement</param>
        /// <returns>Returns a string containing the resulting Insert statement</returns>
        public static string GenInsert(string tableName, IDictionary<string, object> fieldsValues)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidTableException("You must specify a Table Name to perform this operation");

            if (fieldsValues.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            temp.AppendFormat("INSERT INTO {0} ({1}) VALUES ({2})", tableName, GenFieldsEnumeration(fieldsValues.Keys), GenValuesEnumeration(fieldsValues.Values));

            return temp.ToString();
        }

        /// <summary>
        /// Create a SQL Insert statement.
        /// </summary>
        /// <param name="tableName">The table to insert data into</param>
        /// <param name="fields">A collection of strings that will be used as the columns' names in the Insert statement</param>
        /// <param name="values">A collection of objects that will be used as the columns' values in the Insert statement</param>
        /// <returns>Returns a string containing the resulting Insert statement</returns>
        public static string GenInsert(string tableName, IEnumerable<string> fields, IEnumerable<object> values)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidTableException("You must specify a Table Name to perform this operation");

            if (fields.Count() != values.Count())
                throw new DisparityException("The number of fields and values must be equal");

            if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            temp.AppendFormat("INSERT INTO {0} ({1}) VALUES ({2})", tableName, GenFieldsEnumeration(fields), GenValuesEnumeration(values));

            return temp.ToString();
        }

        /// <summary>
        /// Create a SQL Update statement.
        /// </summary>
        /// <param name="tableName">The table to update data into</param>
        /// <param name="fieldsValues">A collection of key/value pairs that will be included in the Update statement</param>
        /// <param name="top">A value indicating the maximum number of columns that may be updated.</param>
        /// <returns>Returns a string containing the resulting Update statement</returns>
        public static string GenUpdate(string tableName, IDictionary<string, object> fieldsValues, int top = 0)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidTableException("You must specify a Table Name to perform this operation");

            if (fieldsValues.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            temp.AppendFormat("UPDATE {0} {1} SET {2}", (top != 0 ? string.Format("TOP({0})", top) : string.Empty), tableName, GenSetPairs(fieldsValues));

            return temp.ToString();
        }

        /// <summary>
        /// Create a SQL Update statement.
        /// </summary>
        /// <param name="tableName">The table to update data in</param>
        /// <param name="fields">A collection of strings that will be used as the columns' names in the Update statement</param>
        /// <param name="values">A collection of objects that will be used as the columns' values in the Update statement</param>
        /// <param name="top">A value indicating the maximum number of columns that may be updated.</param>
        /// <returns>Returns a string containing the resulting Update statement</returns>
        public static string GenUpdate(string tableName, IEnumerable<string> fields, IEnumerable<object> values, int top = 0)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidTableException("You must specify a Table Name to perform this operation");

            if (fields.Count() != values.Count())
                throw new DisparityException("The number of fields and values must be equal");

            if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            temp.AppendFormat("UPDATE {0} {1} SET {2}", (top != 0 ? string.Format("TOP({0})", top) : string.Empty), tableName, GenSetPairs(fields, values));

            return temp.ToString();
        }

        /// <summary>
        /// Create a Delete statment.
        /// </summary>
        /// <param name="tableName">The table to delete data from</param>
        /// <param name="fieldsValues">A collection of key/value pairs that will be included in the Delete statement</param>
        /// <param name="top">A value indicating the maximum number of columns that may be deleted.</param>
        /// <returns>Returns a string containing the resulting Delete statement</returns>
        public static string GenDelete(string tableName, IDictionary<string, object> fieldsValues, int top = 0)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidTableException("You must specify a Table Name to perform this operation");

            if (fieldsValues.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            temp.AppendFormat("DELETE {0} FROM {1} {2}", (top != 0 ? string.Format("TOP({0})", top) : string.Empty), tableName, GenWhere(fieldsValues));

            return temp.ToString();
        }

        /// <summary>
        /// Create a Delete statment.
        /// </summary>
        /// <param name="tableName">The table to delete data from</param>
        /// <param name="fields">A collection of strings that will be used as the columns' names in the Delete statement</param>
        /// <param name="values">A collection of objects that will be used as the columns' values in the Delete statement</param>
        /// <param name="top">A value indicating the maximum number of columns that may be deleted.</param>
        /// <returns>Returns a string containing the resulting Delete statement</returns>
        public static string GenDelete(string tableName, IEnumerable<string> fields, IEnumerable<object> values, int top = 0)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidTableException("You must specify a Table Name to perform this operation");

            if (fields.Count() != values.Count())
                throw new DisparityException("The number of fields and values must be equal");

            if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            temp.AppendFormat("DELETE {0} FROM {1} {2}", (top != 0 ? string.Format("TOP({0})", top) : string.Empty), tableName, GenWhere(fields, values));

            return temp.ToString();
        }

        /// <summary>
        /// Creates a string of comma-separated fields from the provided field array.
        /// </summary>
        /// <param name="fields">The fields' names</param>
        /// <returns>Returns a string containing the field names separated by commas</returns>
        public static string GenFieldsEnumeration(params string[] fields)
        {
            if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            foreach (string item in fields)
            {
                temp.AppendFormat("{0},", item);
            }

            temp = temp.Remove(temp.Length - 1, 1);
            return temp.ToString();
        }

        /// <summary>
        /// Creates a string of comma-separated fields from the provided field array.
        /// </summary>
        /// <param name="fields">The fields' names</param>
        /// <returns>Returns a string containing the field names separated by commas</returns>
        public static string GenFieldsEnumeration(IEnumerable<string> fields)
        {
            if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            foreach (string item in fields)
            {
                temp.AppendFormat("{0},", item);
            }

            temp = temp.Remove(temp.Length - 1, 1);
            return temp.ToString();
        }

        /// <summary>
        /// Creates a string of comma-separated values from the provided values array.
        /// </summary>
        /// <param name="fields">The fields' values</param>
        /// <returns>Returns a string containing the field names separated by commas</returns>
        public static string GenValuesEnumeration(params object[] fields)
        {
            if (fields.Count() <= 0)
                return string.Empty;
            
            StringBuilder temp = new StringBuilder();

            foreach (object item in fields)
            {
                if (item == null)
                {
                    temp.Append("NULL,");
                }
                else if (item is DateTime)
                {
                    temp.AppendFormat("'{0}',", ((DateTime)item).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else if (item.IsNumberType())
                {
                    temp.AppendFormat("{0},", item.ToString());
                }
                else if (item is Enum)
                {
                    temp.AppendFormat("{0},", Convert.ToInt64(item));
                }
                else
                {
                    temp.AppendFormat("'{0}',", item.ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 1, 1);
            return temp.ToString();
        }

        /// <summary>
        /// Creates a string of comma-separated values from the provided values array.
        /// </summary>
        /// <param name="fields">The fields' values</param>
        /// <returns>Returns a string containing the field names separated by commas</returns>
        public static string GenValuesEnumeration(IEnumerable<object> fields)
        {
            if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            foreach (object item in fields)
            {
                if (item == null)
                {
                    temp.Append("NULL,");
                }
                else if (item is DateTime)
                {
                    temp.AppendFormat("'{0}',", ((DateTime)item).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else if (item.IsNumberType())
                {
                    temp.AppendFormat("{0},", item.ToString());
                }
                else if (item is Enum)
                {
                    temp.AppendFormat("{0},", Convert.ToInt64(item));
                }
                else
                {
                    temp.AppendFormat("'{0}',", item.ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 1, 1);
            return temp.ToString();
        }

        /// <summary>
        /// Create a string of assignment pairs of column/values.
        /// </summary>
        /// <param name="fieldsValues">The key/value pairs to use</param>
        /// <returns>Returns a string containing the resulting assignment pairs</returns>
        public static string GenSetPairs(IDictionary<string, object> fieldsValues)
        {
            if (fieldsValues.Count <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            foreach (KeyValuePair<string, object> item in fieldsValues)
            {
                if (item.Value == null)
                {
                    temp.AppendFormat("{0}=NULL,", item.Key);
                }
                else if (item.Value is DateTime)
                {
                    temp.AppendFormat("{0}='{1}',", item.Key, ((DateTime)item.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else if (item.Value.IsNumberType())
                {
                    temp.AppendFormat("{0}={1},", item.Key, item.Value.ToString());
                }
                else if (item.Value is Enum)
                {
                    temp.AppendFormat("{0}={1},", item.Key, Convert.ToInt64(item.Value));
                }
                else
                {
                    temp.AppendFormat("{0}='{1}',", item.Key, item.Value.ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 1, 1);

            return temp.ToString();
        }

        /// <summary>
        /// Create a string of assignment pairs of column/values.
        /// </summary>
        /// <param name="fields">The column names to use</param>
        /// <param name="values">The values to use</param>
        /// <returns>Returns a string containing the resulting assignment pairs</returns>
        public static string GenSetPairs(IEnumerable<string> fields, IEnumerable<object> values)
        {
            if (fields.Count() != values.Count())
                throw new DisparityException("The number of fields and values must be equal");
            else if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder();

            for (int x = 0; x < fields.Count(); x++)
            {
                if (values.ElementAt(x) == null)
                {
                    temp.AppendFormat("{0}=NULL,", fields.ElementAt(x));
                }
                else if (values.ElementAt(x) is DateTime)
                {
                    temp.AppendFormat("{0}='{1}',", fields.ElementAt(x), ((DateTime)values.ElementAt(x)).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else if (values.ElementAt(x).IsNumberType())
                {
                    temp.AppendFormat("{0}={1},", fields.ElementAt(x), values.ElementAt(x).ToString());
                }
                else if (values.ElementAt(x) is Enum)
                {
                    temp.AppendFormat("{0}={1},", fields.ElementAt(x), Convert.ToInt64(values.ElementAt(x)));
                }
                else
                {
                    temp.AppendFormat("{0}='{1}',", fields.ElementAt(x), values.ElementAt(x).ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 1, 1);

            return temp.ToString();
        }

        /// <summary>
        /// Create a WHERE clause comparing keys and values as equal.
        /// </summary>
        /// <param name="fieldsValues">The key/value pairs to use</param>
        /// <param name="includeWhere">Indicates whether the WHERE should be added or not to the resulting clause.</param>
        /// <returns>Returns a string containing the resulting WHERE clause</returns>
        public static string GenWhere(IDictionary<string, object> fieldsValues, bool includeWhere = true)
        {
            if (fieldsValues.Count <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder(includeWhere ? "WHERE " : string.Empty);

            foreach (KeyValuePair<string, object> item in fieldsValues)
            {
                if(item.Value == null)
                {
                    temp.AppendFormat("{0}=NULL AND ", item.Key);
                }
                else if(item.Value is DateTime)
                {
                    temp.AppendFormat("{0}='{1}' AND ", item.Key, ((DateTime)item.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else if(item.Value.IsNumberType())
                {
                    temp.AppendFormat("{0}={1} AND ", item.Key, item.Value.ToString());
                }
                else if (item.Value is Enum)
                {
                    temp.AppendFormat("{0}={1} AND ", item.Key, Convert.ToInt64(item.Value));
                }
                else
                {
                    temp.AppendFormat("{0}='{1}' AND ", item.Key, item.Value.ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 4, 4);

            return temp.ToString();
        }

        /// <summary>
        /// Create a WHERE clause comparing keys and values as equal.
        /// </summary>
        /// <param name="fields">The column names to use</param>
        /// <param name="values">The values to use</param>
        /// <param name="includeWhere">Indicates whether the WHERE should be added or not to the resulting clause.</param>
        /// <returns>Returns a string containing the resulting WHERE clause</returns>
        public static string GenWhere(IEnumerable<string> fields, IEnumerable<object> values, bool includeWhere = true)
        {
            if (fields.Count() != values.Count())
                throw new DisparityException("The number of fields and values must be equal");
            else if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder(includeWhere ? "WHERE " : string.Empty);

            for(int x = 0; x < fields.Count(); x++)
            {
                if (values.ElementAt(x) == null)
                {
                    temp.AppendFormat("{0}=NULL AND ", fields.ElementAt(x));
                }
                else if (values.ElementAt(x) is DateTime)
                {
                    temp.AppendFormat("{0}='{1}' AND ", fields.ElementAt(x), ((DateTime)values.ElementAt(x)).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else if (values.ElementAt(x).IsNumberType())
                {
                    temp.AppendFormat("{0}={1} AND ", fields.ElementAt(x), values.ElementAt(x).ToString());
                }
                else if (values.ElementAt(x) is Enum)
                {
                    temp.AppendFormat("{0}={1} AND ", fields.ElementAt(x), Convert.ToInt64(values.ElementAt(x)));
                }
                else
                {
                    temp.AppendFormat("{0}='{1}' AND ", fields.ElementAt(x), values.ElementAt(x).ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 4, 4);

            return temp.ToString();
        }

        /// <summary>
        /// Create a WHERE clause comparing keys and values as unequal.
        /// </summary>
        /// <param name="fieldsValues">The key/value pairs to use</param>
        /// <param name="includeWhere">Indicates whether the WHERE should be added or not to the resulting clause.</param>
        /// <returns>Returns a string containing the resulting WHERE clause</returns>
        public static string GenWhereDiffered(IDictionary<string, object> fieldsValues, bool includeWhere = true)
        {
            if (fieldsValues.Count <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder(includeWhere ? "WHERE " : string.Empty);

            foreach (KeyValuePair<string, object> item in fieldsValues)
            {
                if (item.Value == null)
                {
                    temp.AppendFormat("{0} != NULL AND ", item.Key);
                }
                else if (item.Value is DateTime)
                {
                    temp.AppendFormat("{0} != '{1}' AND ", item.Key, ((DateTime)item.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else if (item.Value.IsNumberType())
                {
                    temp.AppendFormat("{0} != {1} AND ", item.Key, item.Value.ToString());
                }
                else if (item.Value is Enum)
                {
                    temp.AppendFormat("{0} != {1} AND ", item.Key, Convert.ToInt64(item.Value));
                }
                else
                {
                    temp.AppendFormat("{0} != '{1}' AND ", item.Key, item.Value.ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 4, 4);

            return temp.ToString();
        }

        /// <summary>
        /// Create a WHERE clause comparing keys and values as unequal.
        /// </summary>
        /// <param name="fields">The column names to use</param>
        /// <param name="values">The values to use</param>
        /// <param name="includeWhere">Indicates whether the WHERE should be added or not to the resulting clause.</param>
        /// <returns>Returns a string containing the resulting WHERE clause</returns>
        public static string GenWhereDiffered(IEnumerable<string> fields, IEnumerable<object> values, bool includeWhere = true)
        {
            if (fields.Count() != values.Count())
                throw new DisparityException("The number of fields and values must be equal");
            else if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder(includeWhere ? "WHERE " : string.Empty);

            for (int x = 0; x < fields.Count(); x++)
            {
                if (values.ElementAt(x) == null)
                {
                    temp.AppendFormat("{0} != NULL AND ", fields.ElementAt(x));
                }
                else if (values.ElementAt(x) is DateTime)
                {
                    temp.AppendFormat("{0} != '{1}' AND ", fields.ElementAt(x), ((DateTime)values.ElementAt(x)).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else if (values.ElementAt(x).IsNumberType())
                {
                    temp.AppendFormat("{0} != {1} AND ", fields.ElementAt(x), values.ElementAt(x).ToString());
                }
                else if (values.ElementAt(x) is Enum)
                {
                    temp.AppendFormat("{0} != {1} AND ", fields.ElementAt(x), Convert.ToInt64(values.ElementAt(x)));
                }
                else
                {
                    temp.AppendFormat("{0} != '{1}' AND ", fields.ElementAt(x), values.ElementAt(x).ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 4, 4);

            return temp.ToString();
        }
    }
}
