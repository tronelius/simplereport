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
using SimpleReport.Model.Storage;

namespace SimpleReport.Model
{
    public class ReportManager 
    {
        private readonly IStorage _storage;
        private readonly string _filename;
        public  double _version = 1.0;
        //private ReportDataModel _reportDataModel;

        public ReportManager(IStorage storage)
        {
            _storage = storage;
            //_reportDataModel = _storage.LoadModel();
            //ResolveAllReferences();
        }

        #region Reports
        public IEnumerable<Report> GetReports()
        {
            return _storage.GetReports();
        }


        public Report GetReport(Guid id)
        {
            Report report = _storage.GetReport(id);

            IEnumerable<Parameter> lookupParameters = report.Parameters.Where(s => s.InputType == ParameterInputType.Lookup);
            foreach (Parameter lookupParameter in lookupParameters)
            {
                LookupReport rpt = _storage.GetLookupReport(lookupParameter.LookupReportId);
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
            _storage.SaveReport(reportToSave);
        }
        #endregion

        #region connections
        public IEnumerable<Connection> GetConnections()
        {
            return _storage.GetConnections();
        }

        public Connection Getconnection(Guid id)
        {
            return _storage.GetConnection(id);
        }

        public void SaveConnection(Connection conn)
        {
                _storage.SaveConnection(conn);   
        }

        #endregion

        #region LookupReport
        public IEnumerable<ReportInfo> GetLookupReports()
        {
            return _storage.GetLookupReports();
        }

        public LookupReport GetLookupReport(Guid id)
        {
            return _storage.GetLookupReport(id);
        }

        public void SaveLookupReport(LookupReport rpt)
        {
            _storage.SaveLookupReport(rpt);
        }

        #endregion
    }

   
}
