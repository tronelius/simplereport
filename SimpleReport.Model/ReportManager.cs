using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;

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

    public class ReportManager 
    {
        private readonly IStorage _storage;
        private readonly string _filename;
        public  double _version = 1.0;
        private ReportManagerData _reportManagerData;

        public ReportManager(IStorage storage)
        {
            _storage = storage;
            _reportManagerData = _storage.LoadModel();
            ResolveAllReferences();
        }

        public IEnumerable<Report> GetReports()
        {
            return _reportManagerData.Reports.Cast<Report>();
        }

        public Report GetReport(Guid id)
        {
            Report report = _reportManagerData.Reports.FirstOrDefault(r => r.ID == id);

            IEnumerable<Parameter> lookupParameters = report.Parameters.Where(s => s.InputType == ParameterInputType.Lookup);
            foreach (Parameter lookupParameter in lookupParameters)
            {
                LookupReport rpt = _reportManagerData.LookupReports.FirstOrDefault(r => r.ID == lookupParameter.LookupReportId);
                if (rpt != null) {
                    if (lookupParameter.Choices != null)
                        lookupParameter.Choices.Clear();
                    
                    rpt.Execute().ToList().ForEach(rp => lookupParameter.Choices.Add(rp.Key,rp.Value));
                }
            }

            IEnumerable<Parameter> periodParameters = report.Parameters.Where(s => s.InputType == ParameterInputType.Period);
            foreach (Parameter periodParameter in periodParameters)
            {
                periodParameter.SetDefaultValuesForPeriod();
            }
            return report;
        }

        public void SaveReport(Report reportToSave)
        {
            int index = _reportManagerData.Reports.FindIndex(r => r.ID == reportToSave.ID);
            if (index == -1)
                _reportManagerData.Reports.Add(reportToSave);
            else
                _reportManagerData.Reports[index] = reportToSave;
            _storage.SaveModel(_reportManagerData);
        }

        public void AddConnection(Connection conn)
        {
            _reportManagerData.Connections.Add(conn);
        }

        private void ResolveAllReferences()
        {
            foreach (LookupReport report in _reportManagerData.Reports.OfType<LookupReport>().Union(_reportManagerData.LookupReports))
            {
                Connection conn = _reportManagerData.Connections.FirstOrDefault(c => c.Name == report.ConnectionStringName);
                if (conn == null)
                    throw new Exception(string.Format("Connectionstring {0} not defined in report named: {1}", report.ConnectionStringName, report.Name));
                report.SetConnection(conn);    
            }
        }
       
    }

   
}
