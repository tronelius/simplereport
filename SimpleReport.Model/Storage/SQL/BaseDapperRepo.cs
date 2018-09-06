using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperExtensions;

namespace SimpleReport.Model.Storage.SQL
{
    public class BaseDapperRepo
    {

        protected readonly string _connectionString;

        public BaseDapperRepo() : this(ConfigurationManager.ConnectionStrings["db"].ConnectionString) { }

        public BaseDapperRepo(string connectionstring)
        {
            _connectionString = connectionstring;
        }

        protected T GetFirstResult<T>(string query)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                return cn.Query<T>(query).FirstOrDefault();
            }
        }

        protected T GetFirstResult<T>(string query, object parameters)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                return cn.Query<T>(query, parameters).FirstOrDefault();
            }
        }

        protected IEnumerable<T> GetResults<T>(string query, object parameters)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                return cn.Query<T>(query, parameters);
            }
        }

        protected IEnumerable<T> GetResults<T>(string query)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                return cn.Query<T>(query);
            }
        }

        protected SqlConnection EnsureOpenConnection()
        {
            SqlConnection cn = new SqlConnection(_connectionString);
            cn.Open();
            return cn;
        }

        protected bool Upsert<T>(T obj) where T : class
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var returnval = cn.Update(obj);
                if (returnval)
                    return true;
                
                return cn.Insert(obj)!=null;
            }
        }

        protected int Execute<T>(string query, T param)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                return cn.Execute(query, param);
            }
        }

        protected Guid Insert<T>(string query, T param)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                return cn.Query<Guid>(query, param).First();
            }
        }

        protected void ExecuteInTransaction(params Action<SqlConnection, SqlTransaction>[] commands)
        {
            using (var connection = EnsureOpenConnection())
            using (var transaction = connection.BeginTransaction())//Does a rollback on dispose, so in case of errors it will rollback everything, no need to try catch and do it yourself.
            {
                foreach (var command in commands)
                {
                    command(transaction.Connection, transaction);
                }
                transaction.Commit();
            }
        }
    }
}

