using FluentMigrator;

namespace SimpleReport.Model.Migrations
{
    [Migration(201506181335)]
    public class RenameReportUrlToReportParams : Migration
    {
        public override void Up()
        {
            Rename.Column("ReportUrl").OnTable("Subscription").To("ReportParams");
        }

        public override void Down()
        {
            Rename.Column("ReportParams").OnTable("Subscription").To("ReportUrl");
        }
    }
}
