using System.Collections.Generic;

namespace SimpleReport.Model
{
    public class ReportManagerData
    {
        public List<Connection> Connections;
        public List<Report> Reports;
        public List<LookupReport> LookupReports;

        public ReportManagerData()
        {
            Connections = new List<Connection>();
            Reports = new List<Report>();
            LookupReports = new List<LookupReport>();
        }
    }
}