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
                        connection.Close();
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
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
                        connection.Close();
                        DataTable temp = new DataTable(tableName);
                        temp.Load(reader);
                        return temp;
                    }
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
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
