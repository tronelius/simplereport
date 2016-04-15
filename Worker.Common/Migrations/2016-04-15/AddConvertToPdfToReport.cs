using System;
using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201604151326)]
    public class AddConvertToPdfToReport : Migration
    {
        public override void Up()
        {
            Alter.Table("Report").AddColumn("ConvertToPdf").AsBoolean().WithDefaultValue("false");
        }

        public override void Down()
        {
            Delete.Column("ConvertToPdf").FromTable("Report");
        }
    }
}
