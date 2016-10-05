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
                    query = query.Substring(0, x + 1) + "null" + query.Substring(x + 1);
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
                    query = query.Substring(0, x + 1) + "null" + query.Substring(x + 1);
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
                    query = query.Substring(0, x + 1) + "null" + query.Substring(x + 1);
                }
            }

            using (SqlCommand command = new SqlCommand(query, transaction.Connection))
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

        public static async Task<bool> DoQueryAsync(string query, string connectionString)
        {
            for (int x = 0; x < (query.Length - 1); x++)
            {
                if (query[x] == ',' && query[x + 1] == ',')
                {
                    query = query.Substring(0, x + 1) + "null" + query.Substring(x + 1);
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
                    query = query.Substring(0, x + 1) + "null" + query.Substring(x + 1);
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
                    query = query.Substring(0, x + 1) + "null" + query.Substring(x + 1);
                }
            }

            using (SqlCommand command = new SqlCommand(query, transaction.Connection))
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
    }
}
