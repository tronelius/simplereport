using System;
using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201603111200)]
    public class ReportResultType : Migration
    {
        public override void Up()
        {
            Alter.Table("Report").AddColumn("ReportResultType").AsString(200).Nullable();
            Execute.Sql("Update Report set ReportResultType = 'ExcelResultPlain' where TemplateFormat = 0 or TemplateFormat = 1");
            Execute.Sql("Update Report set ReportResultType = 'WordResultMasterDetail' where TemplateFormat = 2");
        }

        public override void Down()
        {
            Delete.Column("ReportResultType").FromTable("Report");
        }
    }
}
