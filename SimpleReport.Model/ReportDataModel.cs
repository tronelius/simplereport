using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleReport.Model
{
    public class ReportDataModel
    {
        public List<Connection> Connections;
        public List<Report> Reports;
        public List<LookupReport> LookupReports;
        public List<TypeAheadReport> TypeAheadReports;
        public List<Access> AccessLists;
        public Settings Settings;

        public List<string> RemoveIllegalItemsInModel()
        {
            List<string> message = new List<string>();
            var reportErrors = Reports.Where(r => r.ConnectionId == Guid.Empty).ToList();
            reportErrors.ForEach(r =>
            {
                message.Add("Removed Report with empty connection: " + r.Name);
                Reports.Remove(r);
            });
            
            return message;
        } 

        public ReportDataModel()
        {
            Connections = new List<Connection>();
            Reports = new List<Report>();
            LookupReports = new List<LookupReport>();
            TypeAheadReports = new List<TypeAheadReport>();
            AccessLists = new List<Access>();
            Settings = new Settings();
        }
    }
}