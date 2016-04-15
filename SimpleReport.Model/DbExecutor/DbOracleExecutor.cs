using System;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace SimpleReport.Model.DbExecutor
{
    public class DbOracleExecutor : BaseExecutor, IDbExecutor
    {
       public ConnectionVerificationResult VerifyConnectionstring(string connectionString)
        {
            try
            {
                string tempConnString = connectionString;
                if (tempConnString.ToLower().IndexOf("connection timeout=") < 0)
                    tempConnString += ";connection timeout=2";
                using (var conn = new OracleConnection(tempConnString))
                {
                    conn.Open(); // throws if invalid
                }
                return new ConnectionVerificationResult(true, "OK");
            }
            catch (Exception ex)
            {
                return new ConnectionVerificationResult(false, ex.Message);
            }
        }

        public DbParameter CreateStringParameter(string name, int length)
        {
            return new OracleParameter("@" + name, OracleDbType.NVarchar2, 100); //TODO: How does oracle work:P check @ and datatype
        }

        public DbParameter CreateParameter(string key, object value)
        {
            return new OracleParameter(key, value);
        }

        protected override DbConnection GetOpenConnection(Connection conn)
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

        protected override DbDataAdapter GetDataAdapter(DbCommand cmd)
        {
            return new OracleDataAdapter(cmd as OracleCommand);
        }
    }
}