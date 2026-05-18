using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Library_Management_System
{
    public static class DatabaseHelper
    {
        private const string ConnectionString = "Data Source=localhost;Initial Catalog=library;Integrated Security=True;";

        // Get connection
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // Execute Non-Query (Insert, Update, Delete)
        public static bool ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (var p in parameters)
                                cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value));
                        }

                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Execute Scalar (Returns single value)
        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (var p in parameters)
                                cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value));
                        }

                        con.Open();
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Execute Query (Returns DataTable)
        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (var p in parameters)
                                cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value));
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Get DataSet
        public static DataSet ExecuteDataSet(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (var p in parameters)
                                cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value));
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
