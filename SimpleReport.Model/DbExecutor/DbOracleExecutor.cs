using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Oracle.ManagedDataAccess.Client;

namespace SimpleReport.Model.DbExecutor
{
    public class DbOracleExecutor : IDbExecutor
    {
        public List<DataTable> GetMultipleResults(Connection conn, string query, IEnumerable<DbParameter> param)
        {
            throw new NotImplementedException();
        }

        public DataTable GetResults(Connection conn, string query, IEnumerable<DbParameter> param)
        {
            using (OracleConnection cn = GetOpenConnection(conn))
            {
                try
                {
                    OracleCommand cmd = null;
                    DataTable table = new DataTable();
                    cmd = CreateCommand(cn);
                    cmd.CommandType = query.ToLower().StartsWith("select ") ? CommandType.Text : CommandType.StoredProcedure;
                    cmd.CommandText = query;
                    if (param != null)
                        cmd.Parameters.AddRange(param.ToArray());

                    OracleDataAdapter da = null;
                    using (da = new OracleDataAdapter(cmd))
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

        public ConnectionVerificationResult VerifyConnectionstring(string connectionString)
        {
            throw new NotImplementedException();
        }

        private OracleCommand CreateCommand(OracleConnection cn)
        {
            var cmd = cn.CreateCommand();
            cmd.CommandTimeout = 60;
            return cmd;
        }
        private OracleConnection GetOpenConnection(Connection conn)
        {
            OracleConnection cn;
            try
            {
                cn = new OracleConnection(conn.ConnectionString);
                cn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error when opening connection to Database, Name:{0}, Connectionstring:{1}", conn.Name, conn.ConnectionString), ex);
            }
            return cn;
        }
    }
}