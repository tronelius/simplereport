namespace SimpleReport.Model.Subscriptions
{
    public class ReportResult
    {
        public byte[] Data;

        public ReportResult(byte[] data, string fileName)
        {
            Data = data;
            FileName = fileName;
        }

        public string FileName;
    }
}