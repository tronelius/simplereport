﻿using DocumentFormat.OpenXml.Drawing.Charts;
using FluentMigrator;

namespace SimpleReport.Model.Migrations
{
    [Migration(201506230937)]
    public class AddLastRun : Migration
    {
        public override void Up()
        {
            Alter.Table("Subscription").AddColumn("LastRun").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Column("LastRun").FromTable("Subscription");
        }
    }
}
