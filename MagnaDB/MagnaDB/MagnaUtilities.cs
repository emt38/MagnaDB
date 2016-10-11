using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
{
    public enum PresenceBehavior
    {
        IncludeOnly,
        ExcludeAll
    }

    public sealed class MagnaKey
    {
        private IDictionary<string, object> key;

        public MagnaKey(IDictionary<string, object> fieldsValues)
        {
            key = fieldsValues;
        }
    }

    public class Carro
    {
        public int Marca { get; set; }
        public string Modelo { get; set; }
    }

    public static class Utils
    {
        public static MagnaKey MakeKey<T>(this T value, params Expression<Func<T, object>>[] properties) where T : ViewModel<T>, new()
        {
            Dictionary<string, object> fieldsValues = new Dictionary<string, object>();
            
            foreach (Expression<Func<T, object>> item in properties)
            {
                MemberExpression me = item.Body as MemberExpression;
                if (me == null)
                {
                    me = (item.Body as UnaryExpression).Operand as MemberExpression;
                }
                PropertyInfo prop = me.Member as PropertyInfo;
                fieldsValues.Add(prop.Name, prop.GetValue(value));
            }

            return new MagnaKey(fieldsValues);
        }

        public static bool IsNumberType(this object number)
        {
            if (number == null)
                return false;

            return (number is byte || number is short || number is int || number is long ||
                    number is sbyte || number is ushort || number is uint || number is ulong ||
                    number is float || number is double || number is decimal);
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
        public MagnaEventArgs(int nrows, string connectionString)
        {
            RowsAffected = nrows;
            ConnectionString = connectionString;
        }

        public MagnaEventArgs(int nrows, SqlConnection connection)
        {
            RowsAffected = nrows;
            CurrentConnection = connection;
            ConnectionString = connection.ConnectionString;
        }

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
