namespace SimpleReport.Model.Result
{
    public class ResultInfo
    {
        public string ReportResultType { get; set; }
        public string ReportResultName { get; set; }

        public ResultInfo(string reportResultType, string reportResultName)
        {
            ReportResultType = reportResultType;
            ReportResultName = reportResultName;
        }
    }
}