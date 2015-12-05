using System.Collections.Generic;
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
        bool IsInUse(int id);
    }
}