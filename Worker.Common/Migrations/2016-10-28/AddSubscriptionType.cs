using System;
using FluentMigrator;
using Worker.Common.Model;

namespace Worker.Common.Migrations
{
    [Migration(201610280939)]
    public class AddSubscriptionType : Migration
    {
        public override void Up()
        {
            Alter.Table("Subscription").AddColumn("SubscriptionType").AsInt32().WithDefaultValue(0);
            Alter.Table("Schedule").AddColumn("ScheduleType").AsInt32().WithDefaultValue(0);

            Insert.IntoTable("Schedule").Row(new 
            {
                Name = "OneTime",
                ScheduleType = (int)Schedule.ScheduleTypeEnum.Internal,
                Cron = "* * * * *"
            });
        }

        public override void Down()
        {
            Delete.Column("SubscriptionType").FromTable("Subscription");
            Delete.Column("ScheduleType").FromTable("Schedule");

            Delete.FromTable("Schedule").Row(new
            {
                Name = "OneTime",
                ScheduleType = (int)Schedule.ScheduleTypeEnum.Internal
            });
        }
    }
}
