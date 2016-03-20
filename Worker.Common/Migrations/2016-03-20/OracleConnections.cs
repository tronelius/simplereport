using System;
using FluentMigrator;

namespace Worker.Common.Migrations
{
    [Migration(201603201200)]
    public class OracleConnections : Migration
    {
        public override void Up()
        {
            Alter.Table("Connection").AddColumn("ConnectionType").AsInt32().Nullable();
            Execute.Sql("Update Connection set ConnectionType = 1");
            Alter.Table("Connection").AlterColumn("ConnectionType").AsInt32().NotNullable();
        }

        public override void Down()
        {
            Delete.Column("ConnectionType").FromTable("Connection");
        }
    }
}
