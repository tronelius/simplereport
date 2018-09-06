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
            Alter.Table("Report").AlterColumn("Description").AsString(1000).Nullable();
            Alter.Table("Report").AlterColumn("Group").AsString(50).Nullable();
            Alter.Table("Access").AlterColumn("AdGroup").AsString(3000).Nullable();
        }

        public override void Down()
        {
            //no need to go back, bugfix.
        }
    }
}
