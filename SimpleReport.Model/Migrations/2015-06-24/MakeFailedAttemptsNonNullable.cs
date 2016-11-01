using FluentMigrator;

namespace SimpleReport.Model.Migrations
{
    [Migration(201506241529)]
    public class MakeFailedAttemptsNonNullable : Migration
    {
        public override void Up()
        {
            Execute.Sql("Update Subscription set FailedAttempts = 0 where FailedAttempts is null");
            Alter.Column("FailedAttempts").OnTable("Subscription").AsInt32().NotNullable().WithDefaultValue(0);
        }

        public override void Down()
        {
            //no point with a down since the code no longer supports nullable failedattempts.
        }
    }
}
