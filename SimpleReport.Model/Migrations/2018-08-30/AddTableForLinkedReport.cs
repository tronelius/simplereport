using System;
using FluentMigrator;

namespace SimpleReport.Model.Migrations
{
    [Migration(201808301200)]
    public class AddTableForLinkedReport : Migration
    {
        public override void Up()
        {
            Alter.Table("Report").AddColumn("ReportType").AsString(50).NotNullable().WithDefaultValue("SingleReport");
            Alter.Table("Report").AlterColumn("ConnectionId").AsGuid().Nullable();

            Create.Table("LinkedReport")
                .WithColumn("ReportId").AsGuid().NotNullable().ForeignKey("Report", "Id").PrimaryKey()
                .WithColumn("LinkedReportId").AsGuid().NotNullable().ForeignKey("Report", "Id").PrimaryKey()
                .WithColumn("Order").AsInt32().NotNullable();
        }

        public override void Down()
        {
        }
    }
}
