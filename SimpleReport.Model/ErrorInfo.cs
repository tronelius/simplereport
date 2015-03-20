namespace SimpleReport.Model
{
    public class ErrorInfo
    {
        public string ErrorDescription { get; private set; }
        public ErrorLevel Level { get; private set; }

        public ErrorInfo(string errorDescription, ErrorLevel level)
        {
            ErrorDescription = errorDescription;
            Level = level;
        }
    }

    public enum ErrorLevel
    {
        Warning = 1,
        Fatal =2
    }
}