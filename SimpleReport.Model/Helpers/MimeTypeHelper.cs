namespace SimpleReport.Model.Helpers
{
    public static class MimeTypeHelper
    {
        public const string WordMime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        public const string ExcelMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public static bool IsWord(string mime)
        {
            return mime == WordMime;
        }
    }
}