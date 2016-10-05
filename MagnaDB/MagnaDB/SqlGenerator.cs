using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
{
    public static class SqlGenerator
    {
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
                    temp.AppendFormat("'{0}',", ((DateTime)item).ToString("yyyy-MM-dd hh:mm:ss.fff"));
                }
                else if (item.IsNumberType())
                {
                    temp.AppendFormat("{0},", item.ToString());
                }
                else
                {
                    temp.AppendFormat("'{0}',", item.ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 1, 1);
            return temp.ToString();
        }

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
                    temp.AppendFormat("'{0}',", ((DateTime)item).ToString("yyyy-MM-dd hh:mm:ss.fff"));
                }
                else if (item.IsNumberType())
                {
                    temp.AppendFormat("{0},", item.ToString());
                }
                else
                {
                    temp.AppendFormat("'{0}',", item.ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 1, 1);
            return temp.ToString();
        }

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
                    temp.AppendFormat("{0}='{1}',", item.Key, ((DateTime)item.Value).ToString("yyyy-MM-dd hh:mm:ss.fff"));
                }
                else if (item.Value.IsNumberType())
                {
                    temp.AppendFormat("{0}={1},", item.Key, item.Value.ToString());
                }
                else
                {
                    temp.AppendFormat("{0}='{1}',", item.Key, item.Value.ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 1, 1);

            return temp.ToString();
        }

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
                    temp.AppendFormat("{0}='{1}',", fields.ElementAt(x), ((DateTime)values.ElementAt(x)).ToString("yyyy-MM-dd hh:mm:ss.fff"));
                }
                else if (values.ElementAt(x).IsNumberType())
                {
                    temp.AppendFormat("{0}={1},", fields.ElementAt(x), values.ElementAt(x).ToString());
                }
                else
                {
                    temp.AppendFormat("{0}='{1}',", fields.ElementAt(x), values.ElementAt(x).ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 1, 1);

            return temp.ToString();
        }

        public static string GenWhere(IDictionary<string, object> fieldsValues, bool includeWhere = true)
        {
            if (fieldsValues.Count <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder(includeWhere ? "WHERE" : string.Empty);

            foreach (KeyValuePair<string, object> item in fieldsValues)
            {
                if(item.Value == null)
                {
                    temp.AppendFormat("{0}=NULL AND ", item.Key);
                }
                else if(item.Value is DateTime)
                {
                    temp.AppendFormat("{0}='{1}' AND ", item.Key, ((DateTime)item.Value).ToString("yyyy-MM-dd hh:mm:ss.fff"));
                }
                else if(item.Value.IsNumberType())
                {
                    temp.AppendFormat("{0}={1} AND ", item.Key, item.Value.ToString());
                }
                else
                {
                    temp.AppendFormat("{0}='{1}' AND ", item.Key, item.Value.ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 4, 4);

            return temp.ToString();
        }

        public static string GenWhere(IEnumerable<string> fields, IEnumerable<object> values, bool includeWhere = true)
        {
            if (fields.Count() != values.Count())
                throw new DisparityException("The number of fields and values must be equal");
            else if (fields.Count() <= 0)
                return string.Empty;

            StringBuilder temp = new StringBuilder(includeWhere ? "WHERE" : string.Empty);

            for(int x = 0; x < fields.Count(); x++)
            {
                if (values.ElementAt(x) == null)
                {
                    temp.AppendFormat("{0}=NULL AND ", fields.ElementAt(x));
                }
                else if (values.ElementAt(x) is DateTime)
                {
                    temp.AppendFormat("{0}='{1}' AND ", fields.ElementAt(x), ((DateTime)values.ElementAt(x)).ToString("yyyy-MM-dd hh:mm:ss.fff"));
                }
                else if (values.ElementAt(x).IsNumberType())
                {
                    temp.AppendFormat("{0}={1} AND ", fields.ElementAt(x), values.ElementAt(x).ToString());
                }
                else
                {
                    temp.AppendFormat("{0}='{1}' AND ", fields.ElementAt(x), values.ElementAt(x).ToString().Replace("'", "''"));
                }
            }

            temp = temp.Remove(temp.Length - 4, 4);

            return temp.ToString();
        }
    }
}
