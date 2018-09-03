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
using SimpleReport.Model.Constants;
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


            var lookupParameters = new List<Parameter>();
            if (report.ReportType == ReportType.SingleReport)
            {
                lookupParameters = report.Parameters.Where(s => s.InputType == ParameterInputType.Lookup || s.InputType == ParameterInputType.LookupMultipleChoice).ToList();
            } else
            {
                report.Parameters.Clear();
                lookupParameters = new List<Parameter>();
                foreach (var linkedReport in report.ReportList)
                {
                    var fullReport = Storage.GetReport(linkedReport.LinkedReportId);
                    IEnumerable<Parameter> parameters = fullReport.Parameters.Where(s => s.InputType == ParameterInputType.Lookup || s.InputType == ParameterInputType.LookupMultipleChoice);
                    foreach (var param in parameters)
                    {
                        if (lookupParameters.All(p => p.SqlKey != param.SqlKey))
                        {
                            lookupParameters.Add(param);
                        }
                    }

                    var uniqueParameters =
                        fullReport.Parameters.Where(p => report.Parameters.All(r => !r.SqlKey.ToLower().Equals(p.SqlKey.ToLower())));
                    report.Parameters.AddRange(uniqueParameters);
                }
                
            }

            foreach (Parameter lookupParameter in lookupParameters)
            {
                if (lookupParameter.LookupReportId.HasValue)
                {
                    LookupReport rpt = lookupParameter.LookupReport ?? Storage.GetLookupReport(lookupParameter.LookupReportId.Value);
                    if (rpt != null)
                    {
                        lookupParameter.Choices?.Clear();
                        rpt.Execute().ToList().ForEach(rp => lookupParameter.Choices.Add(new KeyValuePair<string, string>(rp.Key, rp.Value)));
                    }
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
