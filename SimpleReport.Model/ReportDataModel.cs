using System.Collections.Generic;

namespace SimpleReport.Model
{
    public class ReportDataModel
    {
        public List<Connection> Connections;
        public List<Report> Reports;
        public List<LookupReport> LookupReports;
        public List<Access> AccessLists;
        public Settings Settings;

        public ReportDataModel()
        {
            Connections = new List<Connection>();
            Reports = new List<Report>();
            LookupReports = new List<LookupReport>();
            AccessLists = new List<Access>();
            Settings = new Settings();
        }
    }
}