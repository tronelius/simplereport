using FluentMigrator;
using Worker.Common.Migrations;

namespace SimpleReport.Model.Migrations
{
    [Migration(201510231440)]
    public class AddSyncedDateToSubscription : Migration
    {
        public override void Up()
        {
            Alter.Table(Tables.Subscription).AddColumn("SyncedDate").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Column("SyncedDate").FromTable(Tables.Subscription);
        }
    }
}
