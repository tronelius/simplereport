﻿using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201506221347)]
    public class AddSendEmptyEmails : Migration
    {
        public override void Up()
        {
            Alter.Table("Subscription").AddColumn("SendEmptyEmails").AsBoolean().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.Column("SendEmptyEmails").FromTable("Subscription");
        }
    }
}
