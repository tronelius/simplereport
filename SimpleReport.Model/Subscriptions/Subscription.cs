using System;
using System.Linq;
using System.Net.Mail;
using NCrontab;

namespace SimpleReport.Model.Subscriptions
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public string ReportParams { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public SubscriptionStatus Status { get; set; }//not int, should be an enum.
        public SubscriptionTypeEnum SubscriptionType { get; set; }
        public DateTime? LastSent { get; set; }
        public DateTime NextSend { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? LastErrorDate { get; set; }
        public int FailedAttempts { get; set; }
        public Guid ScheduleId { get; set; }
        public bool SendEmptyEmails { get; set; }
        public string MailSubject { get; set; }
        public string MailText { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? SyncedDate { get; set; }

        public Subscription()
        {
            Id = default(Guid);
        }
        
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


        public void SetNextSendDate(string schedule)
        {
          var crons = schedule.Split(';'); //we can have composite crons, separated by ;
          var date = crons.Select(CrontabSchedule.Parse).Select(x => x.GetNextOccurrence(DateTime.Now)).Min();

            NextSend = date;
        }
    }

    public enum SubscriptionStatus
    {
        NotSet,
        Success,
        Ongoing,
        Failed,
        Suspended
    }

    public enum SubscriptionTypeEnum
    {
        Default,
        OneTime
    }
}
