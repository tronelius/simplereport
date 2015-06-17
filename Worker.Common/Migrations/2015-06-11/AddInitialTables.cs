using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201506111352)]
    public class AddInitialTables : Migration
    {
        public override void Up()
        {
            Create.Table("Schedule").WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Cron").AsString(255).NotNullable()
                ;

            Create.Table("Subscription").WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("ReportId").AsGuid().NotNullable()
                .WithColumn("ReportUrl").AsString(2000).NotNullable()
                .WithColumn("To").AsString(1000).NotNullable()
                .WithColumn("Cc").AsString(1000).Nullable()
                .WithColumn("Bcc").AsString(1000).Nullable()
                .WithColumn("Status").AsInt32().NotNullable()
                .WithColumn("LastSent").AsDate().Nullable()
                .WithColumn("NextSend").AsDate().NotNullable()
                .WithColumn("ErrorMessage").AsString(2000).Nullable()
                .WithColumn("LastErrorDate").AsDate().Nullable()
                .WithColumn("FailedAttempts").AsInt32().Nullable()
                .WithColumn("ScheduleId").AsInt32().ForeignKey("Schedule", "Id")
                ;
        }

        public override void Down()
        {
            Delete.Table("Subscription");
            Delete.Table("Schedule");
        }
    }
}
