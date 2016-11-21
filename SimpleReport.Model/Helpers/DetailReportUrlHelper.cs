using System;
using System.Web;

namespace SimpleReport.Model.Helpers
{
    public static class DetailReportUrlHelper
    {
        public static string GetUrl(Report report, Report detailReport, string[] headers, string[] r)
        {
            //Home/Report?reportId=c9d8326c-cac8-412e-91ca-f15020222230&productid=1%2C879&syncdate=2008-10-04&selectedAction=onScreen
            var action = detailReport.OnScreenFormatAllowed ? "Report" : "ExecuteReport";

            var url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Home/" + action + "?reportId=" + report.DetailReportId.Value;
            var paramPart = string.Empty;

            detailReport.Parameters.ForEach(p =>
            {
                var key = p.SqlKey.Replace("@", string.Empty);
                var index = Array.IndexOf(headers, key);

                if (index >= 0)
                {
                    var val = r[index];
                    paramPart += $"&{key}={val}";
                }
            });

            url = url + paramPart + (detailReport.OnScreenFormatAllowed ? "&selectedAction=onScreen" : string.Empty);
            return url;
        }
    }
}