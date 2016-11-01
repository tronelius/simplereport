using System;
using FluentMigrator;

namespace SimpleReport.Model.Migrations
{
    [Migration(201603221200)]
    public class AddTypeAhead : Migration
    {
        public override void Up()
        {
            Create.Table("TypeAheadReport")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(300).NotNullable()
                .WithColumn("ConnectionId").AsGuid().NotNullable()
                .WithColumn("SQL").AsString(Int32.MaxValue).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("TypeAheadReport");
        }
    }
}
