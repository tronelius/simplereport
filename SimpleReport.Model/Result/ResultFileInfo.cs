namespace SimpleReport.Model.Result
{
    public class ResultFileInfo
    {
        public ResultFileInfo(string fileName, string mimeType, byte[] data)
        {
            FileName = fileName;
            MimeType = mimeType;
            Data = data;
        }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public byte[] Data { get; set; }

    }
}