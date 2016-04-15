using System;
using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201603221300)]
    public class AddTypeAheadToParameter : Migration
    {
        public override void Up()
        {
            Alter.Table("Parameter").AddColumn("TypeAheadReportId").AsGuid().Nullable().ForeignKey("TypeAhead", "TypeAheadReport", "Id");
        }

        public override void Down()
        {
            Delete.Column("TypeAheadReportId").FromTable("Parameter");
        }
    }
}
