using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperExtensions;
using Worker.Common.Model;

namespace Worker.Common.Repository
{
    public interface ISubscriptionRepository
    {
        Subscription Get(int id);
        int Insert(Subscription schedule);
        List<Subscription> List();
        void Update(Subscription schedule);
        void Delete(int id);
        void SendNow(int id);
    }

    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly string _connectionString;

        public SubscriptionRepository(string connectionstring)
        {
            _connectionString = connectionstring;
        }

        public Subscription Get(int id)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var schedule = cn.Get<Subscription>(id);
                cn.Close();

                return schedule;
            }
        }

        public int Insert(Subscription schedule)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var id = cn.Insert(schedule);
                cn.Close();

                return id;
            }
        }

        public List<Subscription> List()
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var list = cn.GetList<Subscription>();
                cn.Close();

                return list.ToList();
            }
        }

        public void Update(Subscription schedule)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                cn.Update(schedule);
                cn.Close();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                cn.Delete<Subscription>(new { Id = id });
                cn.Close();
            }
        }

        public void SendNow(int id)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                cn.Execute("Update Subscription set NextSend = @date where Id = @Id", new { Date = DateTime.Now, Id = id});
                cn.Close();
            }
        }
    }
}
