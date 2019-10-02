using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReport.Model.Migrations._2019_10_02
{
    [Migration(201910021200)]
    public class AddAutorefresh : Migration
    {
        public override void Up()
        {
            Alter.Table("Report").AddColumn("AutoRefreshAllowed").AsBoolean().NotNullable().WithDefaultValue(false);
            Alter.Table("Report").AddColumn("AutoRefreshIntervalInSeconds").AsInt32().NotNullable().WithDefaultValue(0);
        }

        public override void Down()
        {
            Delete.Column("AutoRefreshAllowed").FromTable("Report");
            Delete.Column("AutoRefreshIntervalInSeconds").FromTable("Report");
        }
    }
}
