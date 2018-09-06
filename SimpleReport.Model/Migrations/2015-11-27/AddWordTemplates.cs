using FluentMigrator;

namespace SimpleReport.Model.Migrations
{
    [Migration(201511272100)]
    public class AddWordTemplates : Migration
    {
        public override void Up()
        {
            Alter.Table("Report").AddColumn("TemplateFormat").AsInt32().NotNullable().WithDefaultValue(0);
            Delete.Column("ResultType").FromTable("Report");
        }

        public override void Down()
        {
            Delete.Column("TemplateFormat").FromTable("Report");
            Alter.Table("Report").AddColumn("ResultType").AsInt32().NotNullable().WithDefaultValue(0);
        }
    }
}
