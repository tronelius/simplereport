using System;
using FluentMigrator;
using Worker.Common.Model;

namespace Worker.Common.Migrations
{
    [Migration(201610281413)]
    public class AddDetailReportIdToReport : Migration
    {
        public override void Up()
        {
            Alter.Table("Report").AddColumn("DetailReportId").AsGuid().Nullable();
        }

        public override void Down()
        {
            Delete.Column("DetailReportId").FromTable("Report");
            }
    }
}
