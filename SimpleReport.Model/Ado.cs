using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReport.Model
{
    public static class ADO
    {
        public static DataTable GetResults<T>(Connection conn, string query, IEnumerable<SqlParameter> param)
        {
            using (SqlConnection cn = GetOpenConnection(conn))
            {
                try
                {
                    SqlCommand cmd = null;
                    DataTable table = new DataTable();
                    cmd = cn.CreateCommand();
                    cmd.CommandType = query.ToLower().StartsWith("select ") ? CommandType.Text : CommandType.StoredProcedure ;
                    cmd.CommandText = query;
                    if (param != null)
                        cmd.Parameters.AddRange(param.ToArray());
                    
                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(table);
                    }
                    return table;
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private static SqlConnection GetOpenConnection(Connection conn)
        {
            SqlConnection cn;
            try
            {
                cn = new SqlConnection(conn.ConnectionString);
                cn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    String.Format("Error when opening connection to Database, Name:{0}, Connectionstring:{1}", conn.Name,
                                  conn.ConnectionString), ex);
            }
            return cn;
        }
    }
}
