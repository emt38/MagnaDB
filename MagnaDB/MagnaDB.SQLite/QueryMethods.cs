using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace MagnaDB.SQLite
{
    /// <summary>
    /// This class includes query and statement execution methods.
    /// </summary>
    public static class QueryMethods
    {
        /// <summary>
        /// Executes a statement onto a database.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connectionString">The Connection String to use.</param>
        /// <returns>Returns a boolean value indicating whether the executed statement affected or not at least one row.</returns>
        public static bool DoQuery(string query, string connectionString)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    int c = command.ExecuteNonQuery();
                    connection.Close();

                    return c > 0;
                }
                catch (SQLiteException ex)
                {
                    connection.Close();

                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a statement onto a database.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connection">An open SQLiteConnection to execute the statement against.</param>
        /// <returns>Returns a boolean value indicating whether the executed statement affected or not at least one row.</returns>
        public static bool DoQuery(string query, SQLiteConnection connection)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    int c = command.ExecuteNonQuery();

                    return c > 0;
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a statement onto a database.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="transaction">An active SQLiteTransaction to execute the statement against.</param>
        /// <returns>Returns a boolean value indicating whether the executed statement affected or not at least one row.</returns>
        public static bool DoQuery(string query, SQLiteTransaction transaction)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    int c = command.ExecuteNonQuery();

                    return c > 0;
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a statement onto a database.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connectionString">The Connection String to use.</param>
        /// <returns>Returns a boolean value indicating whether the executed statement affected or not at least one row.</returns>
        public static async Task<bool> DoQueryAsync(string query, string connectionString)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    int c = await command.ExecuteNonQueryAsync();
                    connection.Close();

                    return c > 0;
                }
                catch (SQLiteException ex)
                {
                    connection.Close();

                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a statement onto a database.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connection">An open SQLiteConnection to execute the statement against.</param>
        /// <returns>Returns a boolean value indicating whether the executed statement affected or not at least one row.</returns>
        public static async Task<bool> DoQueryAsync(string query, SQLiteConnection connection)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    int c = await command.ExecuteNonQueryAsync();

                    return c > 0;
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a statement onto a database.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="transaction">An active SQLiteTransaction to execute the statement against.</param>
        /// <returns>Returns a boolean value indicating whether the executed statement affected or not at least one row.</returns>
        public static async Task<bool> DoQueryAsync(string query, SQLiteTransaction transaction)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    int c = await command.ExecuteNonQueryAsync();

                    return c > 0;
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connectionString">The Connection String to use.</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static object DoScalar(string query, string connectionString)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    return result;
                }
                catch (SQLiteException ex)
                {
                    connection.Close();

                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connection">An open SQLiteConnection to execute the query against.</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static object DoScalar(string query, SQLiteConnection connection)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    object result = command.ExecuteScalar();

                    return result;
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="transaction">An active SQLiteTransaction to execute the query against.</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static object DoScalar(string query, SQLiteTransaction transaction)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    object result = command.ExecuteScalar();

                    return result;
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connectionString">The Connection String to use.</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static async Task<object> DoScalarAsync(string query, string connectionString)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    object result = await command.ExecuteScalarAsync();
                    connection.Close();

                    return result;
                }
                catch (SQLiteException ex)
                {
                    connection.Close();

                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connection">An open SQLiteConnection to execute the query against.</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static async Task<object> DoScalarAsync(string query, SQLiteConnection connection)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    object result = await command.ExecuteScalarAsync();

                    return result;
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="transaction">An active SQLiteTransaction to execute the query against.</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static async Task<object> DoScalarAsync(string query, SQLiteTransaction transaction)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    object result = await command.ExecuteScalarAsync();

                    return result;
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connectionString">The Connection String to use.</param>
        /// <param name="tableName">A name (optional) to assign to the Name property of the resulting DataTable object</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static DataTable TableMake(string query, string connectionString, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        connection.Close();
                        return temp;
                    }
                }
                catch (SQLiteException ex)
                {
                    connection.Close();
                    
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connection">An open SQLiteConnection to execute the query against.</param>
        /// <param name="tableName">A name (optional) to assign to the Name property of the resulting DataTable object</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static DataTable TableMake(string query, SQLiteConnection connection, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
                }
                catch (SQLiteException ex)
                {
                    connection.Close();

                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="transaction">An active SQLiteTransaction to execute the query against.</param>
        /// <param name="tableName">A name (optional) to assign to the Name property of the resulting DataTable object</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static DataTable TableMake(string query, SQLiteTransaction transaction, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connectionString">The Connection String to use.</param>
        /// <param name="tableName">A name (optional) to assign to the Name property of the resulting DataTable object</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static async Task<DataTable> TableMakeAsync(string query, string connectionString, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (IDataReader reader = await command.ExecuteReaderAsync())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        connection.Close();
                        return temp;
                    }
                }
                catch (SQLiteException ex)
                {
                    connection.Close();

                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="connection">An open SQLiteConnection to execute the query against.</param>
        /// <param name="tableName">A name (optional) to assign to the Name property of the resulting DataTable object</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static async Task<DataTable> TableMakeAsync(string query, SQLiteConnection connection, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    using (IDataReader reader = await command.ExecuteReaderAsync())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a scalar result.
        /// </summary>
        /// <param name="query">The query or statement to execute.</param>
        /// <param name="transaction">An active SQLiteTransaction to execute the query against.</param>
        /// <param name="tableName">A name (optional) to assign to the Name property of the resulting DataTable object</param>
        /// <returns>Returns an object with the obtained result (if any) or null if no result is obtained.</returns>
        public static async Task<DataTable> TableMakeAsync(string query, SQLiteTransaction transaction, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    using (IDataReader reader = await command.ExecuteReaderAsync())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.ErrorCode == -1 || ex.ErrorCode == 2 || ex.ErrorCode == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.ErrorCode == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.ErrorCode == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.ErrorCode == 207 /* Invalid Column */)
                    {
                        string msg = string.Format("The {0} property was not found in the table", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
