using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
{
    public static class Utils
    {
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
    /// Representa un Evento para Objetos DBInteractive
    /// </summary>
    /// <param name="sender">La instancia que dispara el evento</param>
    /// <param name="e">Información acerca del evento</param>
    ///
    public delegate void DBIEventHandler(object sender, DBIEventArgs e);

    /// <summary>
    /// This class contains info about the DBI event context
    /// </summary>
    public class DBIEventArgs : EventArgs
    {
        public DBIEventArgs(int nrows, string connectionString)
        {
            RowsAffected = nrows;
            ConnectionString = connectionString;
        }

        public DBIEventArgs(int nrows, SqlConnection connection)
        {
            RowsAffected = nrows;
            CurrentConnection = connection;
            ConnectionString = connection.ConnectionString;
        }

        public DBIEventArgs(int nrows, SqlTransaction transaction)
        {
            List<DataModel<>> models = new List<DataModel<T>>();
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
