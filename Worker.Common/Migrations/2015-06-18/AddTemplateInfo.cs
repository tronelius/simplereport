using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201506181228)]
    public class AddTemplateInfo : Migration
    {
        public override void Up()
        {
            Alter.Table("Subscription").AddColumn("MailSubject").AsString().Nullable();
            Alter.Table("Subscription").AddColumn("MailText").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Column("MailSubject").FromTable("Subscription");
            Delete.Column("MailText").FromTable("Subscription");
        }
    }
}
