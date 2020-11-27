using System;
using System.Collections.Generic;

namespace SimpleReport.Model.Subscriptions
{
    public interface IScheduleRepository
    {
        Schedule Get(Guid id);
        Guid Insert(Schedule schedule);
        List<Schedule> List();

        List<Schedule> ListAll();
        void Update(Schedule schedule);
        void Delete(Guid id);
        bool IsInUse(Guid id);
        Schedule GetOneTimeSchedule();
    }
}