using System.Collections.Generic;

namespace SimpleReport.Model.Subscriptions
{
    public interface IScheduleRepository
    {
        Schedule Get(int id);
        int Insert(Schedule schedule);
        List<Schedule> List();

        List<Schedule> ListAll();
        void Update(Schedule schedule);
        void Delete(int id);
        bool IsInUse(int id);
        Schedule GetOneTimeSchedule();
    }
}