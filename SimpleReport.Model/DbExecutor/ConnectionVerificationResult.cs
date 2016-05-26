namespace SimpleReport.Model.DbExecutor
{
    public struct ConnectionVerificationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ConnectionVerificationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}