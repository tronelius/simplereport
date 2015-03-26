using System.Collections.Generic;

namespace SimpleReport.Model
{
    public class ReportDataModel
    {
        public List<Connection> Connections;
        public List<Report> Reports;
        public List<LookupReport> LookupReports;

        public ReportDataModel()
        {
            Connections = new List<Connection>();
            Reports = new List<Report>();
            LookupReports = new List<LookupReport>();
        }
    }
}