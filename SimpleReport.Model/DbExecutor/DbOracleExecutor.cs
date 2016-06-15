using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Oracle.ManagedDataAccess.Client;

namespace SimpleReport.Model.DbExecutor
{
    public class DbOracleExecutor : BaseExecutor, IDbExecutor
    {
        public override List<DataTable> GetMultipleResults(Connection conn, string query, IEnumerable<DbParameter> param)
        {
            var paramList = param.ToList();
            var parsedQuery = ParseQuery(query, paramList);
            var parsedParamList = ParseParameters(paramList);
            return base.GetMultipleResults(conn, parsedQuery, parsedParamList);
        }

        public override DataTable GetResults(Connection conn, string query, IEnumerable<DbParameter> param)
        {
            var paramList = param.ToList();
            var parsedQuery = ParseQuery(query, paramList);
            var parsedParamList = ParseParameters(paramList);
            return base.GetResults(conn, parsedQuery, parsedParamList);
        }

        private List<DbParameter> ParseParameters(List<DbParameter> paramList)
        {
            foreach (var dbParameter in paramList)
            {
                dbParameter.ParameterName = dbParameter.ParameterName.Replace("@", ":");
            }
            return paramList;
        }

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
            return new OracleParameter(name, OracleDbType.NVarchar2, 100);
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

        //The sql-editor clientside is based on @ to extract parameters. Oracle uses :param instead of @param, so we try to change all parameters to the oracle-name here.
        private string ParseQuery(string query, List<DbParameter> param)
        {
            var q = query;
            foreach (var dbParameter in param)
            {
                q = q.Replace(dbParameter.ParameterName, ":" + dbParameter.ParameterName.Replace("@",""));//TODO: verify.
            }

            return q;
        }
    }
}