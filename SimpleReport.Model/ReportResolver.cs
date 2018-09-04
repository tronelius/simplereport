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



            if (report.ReportType == ReportType.MultiReport)
            {
                report.Parameters.Clear();
                foreach (var linkedReport in report.ReportList)
                {
                    var fullReport = Storage.GetReport(linkedReport.LinkedReportId);

                    var uniqueParameters = new List<Parameter>();
                    foreach (var param in fullReport.Parameters)
                    {
                        var existingParameter = report.Parameters.Find(par => par.SqlKey == param.SqlKey);
                        if (existingParameter != null)
                        {
                            if (existingParameter.InputType != param.InputType)
                            {
                                existingParameter.SqlKey += "_" + existingParameter.InputType;
                                param.SqlKey += "_" + param.InputType;
                                uniqueParameters.Add(param);
                            }
                        }
                        else
                        {
                            uniqueParameters.Add(param);
                        }
                    }
                    report.Parameters.AddRange(uniqueParameters);
                }
            }
            var lookupParameters = report.Parameters.Where(s => s.InputType == ParameterInputType.Lookup || s.InputType == ParameterInputType.LookupMultipleChoice);

            foreach (var lookupParameter in lookupParameters)
            {
                if (lookupParameter.LookupReportId.HasValue)
                {
                    var rpt = lookupParameter.LookupReport ?? Storage.GetLookupReport(lookupParameter.LookupReportId.Value);
                    if (rpt != null)
                    {
                        lookupParameter.Choices?.Clear();
                        rpt.Execute().ToList().ForEach(rp => lookupParameter.Choices.Add(new KeyValuePair<string, string>(rp.Key, rp.Value)));
                    }
                }
            }

            var periodParameters = report.Parameters.Where(s => s.InputType == ParameterInputType.Period);
            foreach (var periodParameter in periodParameters)
            {
                periodParameter.SetDefaultValuesForPeriod();
            }

            var parameterNames = report.Parameters.Select(p => p.SqlKey);
            return report;
        }


    }


}
