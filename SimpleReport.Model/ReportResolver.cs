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
using SimpleReport.Model.Exceptions;
using SimpleReport.Model.Storage;

namespace SimpleReport.Model
{
    public class ReportResolver 
    {
        public readonly IStorage Storage;
        public ReportResolver(IStorage storage)
        {
            Storage = storage;
        }
        
        public Report GetReport(Guid id)
        {
            Report report = Storage.GetReport(id);
            if (report == null)
                throw new EntityNotFoundException("Report not found");

            IEnumerable<Parameter> lookupParameters = report.Parameters.Where(s => s.InputType == ParameterInputType.Lookup);
            foreach (Parameter lookupParameter in lookupParameters)
            {
                LookupReport rpt = Storage.GetLookupReport(lookupParameter.LookupReportId);
                if (rpt != null) {
                    if (lookupParameter.Choices != null)
                        lookupParameter.Choices.Clear();
                    rpt.Execute().ToList().ForEach(rp => lookupParameter.Choices.Add(new KeyValuePair<string, string>(rp.Key,rp.Value)));
                }
            }

            IEnumerable<Parameter> periodParameters = report.Parameters.Where(s => s.InputType == ParameterInputType.Period);
            foreach (Parameter periodParameter in periodParameters)
            {
                periodParameter.SetDefaultValuesForPeriod();
            }
            return report;
        }
    }

   
}
