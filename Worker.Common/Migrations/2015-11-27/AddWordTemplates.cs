using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201511272100)]
    public class AddWordTemplates : Migration
    {
        public override void Up()
        {
            Alter.Table("Report").AddColumn("HasWordTemplate").AsBoolean().NotNullable().WithDefault(0);
            Delete.Column("ResultType").FromTable("Report");
        }

        public override void Down()
        {
            Delete.Column("HasWordTemplate").FromTable("Report");
            Alter.Table("Report").AddColumn("ResultType").AsInt32().NotNullable().WithDefault(0);
        }
    }
}
