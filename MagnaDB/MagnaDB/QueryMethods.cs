using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace MagnaDB
{
    public static class QueryMethods
    {
        public static bool DoQuery(string query, string connectionString)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    int c = command.ExecuteNonQuery();
                    connection.Close();

                    return c > 0;
                }
                catch (SqlException ex)
                {
                    connection.Close();

                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static bool DoQuery(string query, SqlConnection connection)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    int c = command.ExecuteNonQuery();

                    return c > 0;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static bool DoQuery(string query, SqlTransaction transaction)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    int c = command.ExecuteNonQuery();

                    return c > 0;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static async Task<bool> DoQueryAsync(string query, string connectionString)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    int c = await command.ExecuteNonQueryAsync();
                    connection.Close();

                    return c > 0;
                }
                catch (SqlException ex)
                {
                    connection.Close();

                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static async Task<bool> DoQueryAsync(string query, SqlConnection connection)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    int c = await command.ExecuteNonQueryAsync();

                    return c > 0;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static async Task<bool> DoQueryAsync(string query, SqlTransaction transaction)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    int c = await command.ExecuteNonQueryAsync();

                    return c > 0;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static object DoScalar(string query, string connectionString)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    return result;
                }
                catch (SqlException ex)
                {
                    connection.Close();

                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static object DoScalar(string query, SqlConnection connection)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    object result = command.ExecuteScalar();

                    return result;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static object DoScalar(string query, SqlTransaction transaction)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    object result = command.ExecuteScalar();

                    return result;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static async Task<object> DoScalarAsync(string query, string connectionString)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    object result = await command.ExecuteScalarAsync();
                    connection.Close();

                    return result;
                }
                catch (SqlException ex)
                {
                    connection.Close();

                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static async Task<object> DoScalarAsync(string query, SqlConnection connection)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    object result = await command.ExecuteScalarAsync();

                    return result;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static async Task<object> DoScalarAsync(string query, SqlTransaction transaction)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    object result = await command.ExecuteScalarAsync();

                    return result;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static DataTable TableMake(string query, string connectionString, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        connection.Close();
                        return temp;
                    }
                }
                catch (SqlException ex)
                {
                    connection.Close();
                    
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static DataTable TableMake(string query, SqlConnection connection, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
                }
                catch (SqlException ex)
                {
                    connection.Close();

                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static DataTable TableMake(string query, SqlTransaction transaction, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static async Task<DataTable> TableMakeAsync(string query, string connectionString, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        connection.Close();
                        return temp;
                    }
                }
                catch (SqlException ex)
                {
                    connection.Close();

                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static async Task<DataTable> TableMakeAsync(string query, SqlConnection connection, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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

        public static async Task<DataTable> TableMakeAsync(string query, SqlTransaction transaction, string tableName = "")
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Insert(x + 1, "NULL");
                }
            }

            using (SqlCommand command = new SqlCommand(query, transaction.Connection))
            {
                command.Transaction = transaction;
                try
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208 /* Invalid Table */)
                    {
                        string msg = string.Format("The {0} table was not found in the database", ex.Message.Substring(ex.Message.IndexOf("'")));
                        msg = msg.Remove(msg.Length - 1);
                        throw new InvalidModelException(msg, ex);
                    }
                    if (ex.Number == -1 || ex.Number == 2 || ex.Number == 53 /* Server not Found */)
                        throw new DbConnectionException("The Database Server was not found, please check the Server Configurations or change the connection string to solve this issue.", ex);
                    if (ex.Number == -2 /* Timeout Expired */)
                        throw new DbConnectionException("The timeout expired while establishing a connection to the server", ex);
                    if (ex.Number == 18456)
                        throw new DbConnectionException("The Login failed for the specified user");
                    if (ex.Number == 207 /* Invalid Column */)
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
