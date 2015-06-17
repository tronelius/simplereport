using System;
using System.Net.Mail;

namespace Worker.Common.Model
{
    public class Subscription
    {
        public int Id { get; set; }
        public Guid ReportId { get; set; }
        public string ReportUrl { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public int Status { get; set; }//not int, should be an enum.
        public DateTime? LastSent { get; set; }
        public DateTime NextSend { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? LastErrorDate { get; set; }
        public int? FailedAttempts { get; set; }
        public int ScheduleId { get; set; }

        public string Validate()
        {
            if (string.IsNullOrWhiteSpace(To + Cc + Bcc))
                return "At least one recipient must be defined";

            var emails = To + ";" + Cc + ";" + Bcc;
            string[] allToAddresses = emails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string toAddress in allToAddresses)
            {
                try
                {
                    new MailAddress(toAddress);
                }
                catch (FormatException)
                {
                    return "Malformed email-recipients";
                }
            }

            return null;
        }
    }
}
