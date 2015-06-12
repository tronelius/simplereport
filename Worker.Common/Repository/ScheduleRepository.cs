using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DapperExtensions;
using Worker.Common.Model;

namespace Worker.Common.Repository
{
    public interface IScheduleRepository
    {
        Schedule Get(int id);
        int Insert(Schedule schedule);
        List<Schedule> List();
        void Update(Schedule schedule);
        void Delete(int id);
    }

    public class ScheduleRepository : IScheduleRepository
    {
        private readonly string _connectionString;

        public ScheduleRepository(string connectionstring)
        {
            _connectionString = connectionstring;
        }

        public Schedule Get(int id)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var schedule = cn.Get<Schedule>(id);
                cn.Close();

                return schedule;
            }
        }

        public int Insert(Schedule schedule)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var id = cn.Insert(schedule);
                cn.Close();

                return id;
            }
        }

        public List<Schedule> List()
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var list = cn.GetList<Schedule>();
                cn.Close();

                return list.ToList();
            }
        }

        public void Update(Schedule schedule)
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
                cn.Delete<Schedule>(new { Id = id});
                cn.Close();
            }
        }
    }
}
