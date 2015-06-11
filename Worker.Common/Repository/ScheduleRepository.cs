using System.Data.SqlClient;
using DapperExtensions;
using Worker.Common.Model;

namespace Worker.Common.Repository
{
    public interface IScheduleRepository
    {
        Schedule Get(int id);
        int Insert(Schedule schedule);
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
    }
}
