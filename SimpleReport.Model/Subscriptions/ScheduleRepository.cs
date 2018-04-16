using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DapperExtensions;
using SimpleReport.Model.Storage.SQL;

namespace SimpleReport.Model.Subscriptions
{
    public class ScheduleRepository : BaseDapperRepo, IScheduleRepository
    {
        public ScheduleRepository(string connectionstring) : base(connectionstring){}

        public Schedule Get(int id)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var schedule = cn.Get<Schedule>(id);
                return schedule;
            }
        }

        public int Insert(Schedule schedule)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var id = cn.Insert(schedule);
                return id;
            }
        }

        public List<Schedule> List()
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var list = cn.GetList<Schedule>().Where(a => a.ScheduleType == 0);
                return list.ToList();
            }
        }

        public List<Schedule> ListAll()
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                return cn.GetList<Schedule>().ToList();
            }
        }

        public void Update(Schedule schedule)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                cn.Update(schedule);
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                cn.Delete<Schedule>(new { Id = id});
            }
        }

        public bool IsInUse(int id)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var predicate = Predicates.Field<Subscription>(f => f.ScheduleId, Operator.Eq, id);
                return cn.Count<Subscription>(predicate) > 0;
            }
        }

        public Schedule GetOneTimeSchedule()
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                return cn.GetList<Schedule>(new { Name = "OneTime", ScheduleType = (int)Schedule.ScheduleTypeEnum.Internal}).First();
            }
        }
    }
}
