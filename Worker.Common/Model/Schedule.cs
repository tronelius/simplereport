namespace Worker.Common.Model
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cron { get; set; }
        public ScheduleTypeEnum ScheduleType { get; set; }

        public enum ScheduleTypeEnum
        {
            Default,
            Internal
        }
    }
}
