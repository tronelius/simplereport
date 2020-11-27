using System;

namespace SimpleReport.Model.Subscriptions
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Cron { get; set; }
        public ScheduleTypeEnum ScheduleType { get; set; }

        public Schedule()
        {
            Id = Guid.NewGuid();
        }
        
        public enum ScheduleTypeEnum
        {
            Default,
            Internal
        }
    }
}
