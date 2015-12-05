using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleReport.Model;
using SimpleReport.Model.Storage;
using SimpleReport.Model.Storage.SQL;
using Worker.Common.Migrations;

namespace SimpleReportTests
{
    [TestFixture]
    public class ReportStorageTests
    {
        string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\LocalTest.mdf;Integrated Security=True;Connect Timeout=30";
        private IStorage sqlStorage;
        private IStorageHelper _storageHelper = new StorageHelper();
        private Guid _connId = new Guid("4A244400-139F-401E-B82F-532815E77746");
        private Guid _reportguid = new Guid("07E8C99C-4CEF-4A15-A837-5C0F74E1DEBC");
        private Guid _lookupreportguid = new Guid("96DF80D6-60B4-42C1-9ED0-B34923D0E3D9");
        private Guid _accessId  = new Guid("171D992A-8C56-481B-BD66-7372696B0058");

        [SetUp]
        public void Setup()
        {
            var migration = new Migrator(constr);
            migration.Up();
            sqlStorage = new SQLStorage(constr, _storageHelper);
        }

        [Test]
        public void StoreModelFromCode()
        {
            Settings settings = GetSettings();
            Connection conn = GetConnection();
            Access acc = GetAccessList();
            LookupReport lrpt = GetLookupReport();
            Report report = GetReport();

            sqlStorage.SaveSettings(settings);
            sqlStorage.SaveConnection(conn);
            sqlStorage.SaveAccessList(acc);
            sqlStorage.SaveLookupReport(lrpt);
            sqlStorage.SaveReport(report);

            Report fromdb = sqlStorage.GetReport(report.Id);
            Settings settingsfromdb = sqlStorage.GetSettings();
            AreEqualByJson(report, fromdb);
            AreEqualByJson(settings, settingsfromdb);
        }

        private Settings GetSettings()
        {
            return new Settings() {AdminAccess = "utvecklare;developers"};
        }

        private Report GetReport()
        {
            Report rpt = new Report();
            rpt.Id = _reportguid;
            rpt.TemplateFormat = TemplateFormat.Empty;
            rpt.MailSubject = "Testsubject";
            rpt.MailText = "TestText";
            rpt.OnScreenFormatAllowed = true;
            rpt.ReportOwnerAccessId = Guid.Empty;
            rpt.TemplateFormat = TemplateFormat.Excel;
            rpt.SubscriptionAccessStyle = AccessStyle.Anyone;
            rpt.TemplateEditorAccessStyle = AccessStyle.Anyone;
            rpt.AccessId = _accessId;
            rpt.Connection = GetConnection();
            rpt.ConnectionId = _connId;
            rpt.Description = "TestDescription";
            rpt.Group = "TestGroup";
            rpt.Name = "TestName";
            rpt.Sql = "Select * from test";
            rpt.Parameters = new ParameterList();
            rpt.Parameters.Add(new Parameter() {HelpText = "helptext", InputType = ParameterInputType.Integer, Label = "helpLabel", LookupReportId = null,SqlKey = "@key", Mandatory = true, Value = "defaultValue", ReportId = _reportguid});
            rpt.Parameters.Add(new Parameter() {HelpText = "Lookuphelptext", InputType = ParameterInputType.Lookup,  LookupReportId=_lookupreportguid, Label = "lookuphelpLabel", SqlKey = "@lookupkey", Mandatory = false, Value = "lookupDefaultValue", ReportId = _reportguid });
            return rpt;
        }

        private Connection GetConnection()
        {
            return new Connection(_connId, "connection","connectionstring");
        }

        private LookupReport GetLookupReport()
        {
            return new LookupReport() {ConnectionId = _connId, Id=_lookupreportguid, Name = "lookupreporttest", Sql = "select * from special_table"};
        }

        private Access GetAccessList()
        {
            return new Access() {ADGroup = "utvecklare;Developers", Name="Only developers", Id=_accessId};
        }

        public static void AreEqualByJson(object expected, object actual)
        {
            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}
