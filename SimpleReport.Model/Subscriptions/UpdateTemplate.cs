using System;

namespace SimpleReport.Model.Subscriptions
{
    public class UpdateTemplateText
    {
        public Guid ReportGuid { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }
}
