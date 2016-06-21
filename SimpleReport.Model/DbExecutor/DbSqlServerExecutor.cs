﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using SimpleReport.Model.Replacers;

namespace SimpleReport.Model.DbExecutor
{
    public class DbSqlServerExecutor : BaseExecutor, IDbExecutor
    {
        public DbSqlServerExecutor(IReplacer replacer) : base(replacer)
        {
        }

        public ConnectionVerificationResult VerifyConnectionstring(string connectionString)
        {
            try
            {
                string tempConnString = connectionString;
                if (tempConnString.ToLower().IndexOf("connection timeout=") < 0)
                    tempConnString += ";connection timeout=2";
                using (SqlConnection conn = new SqlConnection(tempConnString))
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
            return new SqlParameter("@" + name, SqlDbType.NVarChar, length);
        }

        public DbParameter CreateParameter(string key, object value)
        {
            return new SqlParameter(key, value);
        }

        protected override DbConnection GetOpenConnection(Connection conn)
        {
            SqlConnection cn;
            try
            {
                cn = new SqlConnection(conn.ConnectionString);
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
            return new SqlDataAdapter(cmd as SqlCommand);
        }

     
    }
}
