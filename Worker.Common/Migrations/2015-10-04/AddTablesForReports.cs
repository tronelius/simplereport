using System;
using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201510041200)]
    public class AddTablesForReports : Migration
    {
        public override void Up()
        {
            Create.Table("Connection").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("Verified").AsBoolean().NotNullable()
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("ConnectionString").AsString(1000).NotNullable();

            Create.Table("LookupReport").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("Name").AsString(300).NotNullable()
                .WithColumn("ConnectionId").AsGuid().NotNullable().ForeignKey("Connection","Id")
                .WithColumn("SQL").AsString(Int32.MaxValue).NotNullable();

            Create.Table("Report").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
               .WithColumn("Name").AsString(500).NotNullable()
               .WithColumn("Group").AsString(50).NotNullable()
               .WithColumn("Description").AsString(1000).NotNullable()
               .WithColumn("AccessId").AsGuid().Nullable()
               .WithColumn("ConnectionId").AsGuid().NotNullable().ForeignKey("Connection", "Id")
               .WithColumn("SQL").AsString(Int32.MaxValue).NotNullable()
               .WithColumn("ResultType").AsInt32().NotNullable()
               .WithColumn("Template").AsByte().Nullable()
               .WithColumn("MailSubject").AsString(1000).Nullable()
               .WithColumn("MailText").AsString(Int32.MaxValue).Nullable()
               .WithColumn("ReportOwnerAccessId").AsGuid().Nullable()
               .WithColumn("OnScreenFormatAllowed").AsBoolean().NotNullable()
               .WithColumn("TemplateEditorAccessStyle").AsInt32().NotNullable()
               .WithColumn("SubscriptionAccessStyle").AsInt32().NotNullable();

            Create.Table("Parameter")
                .WithColumn("SqlKey").AsString(255).NotNullable().PrimaryKey()
                .WithColumn("ReportId").AsGuid().NotNullable().ForeignKey("Report", "Id").PrimaryKey()
                .WithColumn("Label").AsString(500).NotNullable()
                .WithColumn("Value").AsString(1000).Nullable()
                .WithColumn("InputType").AsInt32().NotNullable()
                .WithColumn("Mandatory").AsBoolean().NotNullable()
                .WithColumn("HelpText").AsString(2000).NotNullable()
                .WithColumn("LookupReportId").AsGuid().Nullable().ForeignKey("LookupReport", "Id");
                
            Create.Table("Access").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("Name").AsString(1000).NotNullable()
                .WithColumn("ADGroup").AsString(3000).NotNullable();

            Create.Table("Settings").WithColumn("Name").AsString(500).PrimaryKey().NotNullable()
                .WithColumn("Value").AsString(3500).Nullable();

            Create.Index("ConnectionId_LookupReport").OnTable("LookupReport").OnColumn("ConnectionId");
            Create.Index("ConnectionId_Report").OnTable("Report").OnColumn("ConnectionId");
            Create.Index("ReportID_Report").OnTable("Parameter").OnColumn("ReportId");

        }

        public override void Down()
        {
            Delete.Index("ConnectionId_LookupReport").OnTable("LookupReport");
            Delete.Index("ConnectionId_Report").OnTable("Report");
            Delete.Index("ReportID_Report").OnTable("Parameter");
            Delete.Table("Parameter");
            Delete.Table("Report");
            Delete.Table("LookupReport");
            Delete.Table("Connection");
            Delete.Table("Access");
            Delete.Table("Settings");
        }
    }
}
