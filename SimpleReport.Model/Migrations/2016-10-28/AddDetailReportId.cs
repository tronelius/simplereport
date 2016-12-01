using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;


namespace SimpleReport.Model.Migrations
{
    [Migration(201610281413)]
    public class AddDetailReportIdToReport : Migration
    {
        public override void Up()
        {
            Alter.Table("Report").AddColumn("DetailReportId").AsGuid().Nullable();
        }

        public override void Down()
        {
            Delete.Column("DetailReportId").FromTable("Report");
        }
    }
}

