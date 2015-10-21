using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleReport.Model;
using SimpleReport.Model.Storage;
using Worker.Common.Migrations;

namespace SimpleReportTests
{
    [TestFixture]
    public class ReportStorageTests
    {
        string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\LocalTest.mdf;Integrated Security=True;Connect Timeout=30";
        private IStorage sqlStorage;
        private Guid connId = new Guid("4A244400-139F-401E-B82F-532815E77746");

        [SetUp]
        public void Setup()
        {
            var migration = new Migrator(constr);
            migration.Up();
            sqlStorage = new SQLStorage(constr);
        }

        [Test]
        public void ConvertModelFromFileToDB()
        {
            
        }

        [Test][Ignore]
        public void InsertCompleteModel()
        {
            Report report = GetTestReport();
            Connection conn = GetTestConnection();
            sqlStorage.SaveConnection(conn);
            sqlStorage.SaveReport(report);
            Report fromdb = sqlStorage.GetReport(report.Id);
            Assert.That(report, Is.EqualTo(fromdb));
        }

        private Report GetTestReport()
        {
            Report rpt = new Report();
            rpt.Id = new Guid("07E8C99C-4CEF-4A15-A837-5C0F74E1DEBC");
            rpt.HasTemplate = false;
            rpt.MailSubject = "Testsubject";
            rpt.MailText = "TestText";
            rpt.OnScreenFormatAllowed = true;
            rpt.ReportOwnerAccessId = Guid.Empty;
            rpt.ResultType = ResultType.SimpleExcel;
            rpt.SubscriptionAccessStyle = AccessStyle.Anyone;
            rpt.TemplateEditorAccessStyle = AccessStyle.Anyone;
            rpt.AccessId = null;
            rpt.ConnectionId = connId;
            rpt.Description = "TestDescription";
            rpt.Group = "TestGroup";
            rpt.Name = "TestName";
            rpt.Sql = "Select * from test";
            return rpt;
        }

        private Connection GetTestConnection()
        {
            return new Connection(connId, "connection","connectionstring");
        }
    }
}
