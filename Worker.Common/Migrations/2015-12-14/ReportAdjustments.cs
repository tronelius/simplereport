using System;
using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201512142100)]
    public class ReportAdjustments : Migration
    {
        public override void Up()
        {
            Alter.Table("Report").AlterColumn("Template").AsBinary(Int32.MaxValue).Nullable();
        }

        public override void Down()
        {
            //no need to go back, bugfix.
        }
    }
}
