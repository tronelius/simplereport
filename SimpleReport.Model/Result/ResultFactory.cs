using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleReport.Model.Constants;

namespace SimpleReport.Model.Result
{
    public static class ResultFactory
    {

        public static Result GetInstance(Report report, Template template)
        {
            //Little bit ugly, List from Ninject would be nicer...
            switch (report.ReportResultType)
            {
                case "ExcelResultPlain":
                    return new ExcelResultPlain(report, template);
                case "WordResultPlain":
                    return new WordResultPlain(report, template);
                case "WordResultMasterDetail":
                    return new WordResultMasterDetail(report, template);
                default:
                    return new ExcelResultPlain(report, template);
            }
           
        }

        public static Result GetNextResult(Report report, Template template)
        {
            if (report.TemplateFormat == TemplateFormat.Word)
            {
                if (report.IsMasterDetailReport())
                    return new WordResultMasterDetail(report, template);
                return new WordResultPlain(report, template);
            }
            return new ExcelResultPlain(report, template);
        }

        public static List<ResultInfo> GetList()
        {
            List<ResultInfo> resulttypes = new List<ResultInfo>();
            resulttypes.Add(new ExcelResultPlain().ResultInfo);
            resulttypes.Add(new WordResultPlain().ResultInfo);
            resulttypes.Add(new WordResultMasterDetail().ResultInfo);
            return resulttypes;
        }
    }
}
