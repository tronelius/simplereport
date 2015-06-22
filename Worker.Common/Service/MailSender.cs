using System;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace Worker.Common.Service
{
    public interface IMailSender
    {
        void Send(string to, string cc, string bcc, string mailSubject, string mailText, byte[] reportData);
    }

    public class MailSender : IMailSender
    {
        public void Send(string to, string cc, string bcc, string mailSubject, string mailText, byte[] data)
        {
            var client = GetClient();

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("Test@test.se");

            AddRecipients(to, mail.To.Add);
            AddRecipients(cc, mail.CC.Add);
            AddRecipients(bcc, mail.Bcc.Add);

            mail.Subject = mailSubject;
            mail.Body = mailText;

            if (data != null && data.Length > 0)
            {
                using (MemoryStream memStream = new MemoryStream(data))
                {
                    mail.Attachments.Add(new Attachment(memStream, "report.xlsx"));
                    client.Send(mail);
                }
            }
            else
            {
                client.Send(mail);
            }
        }

        private void AddRecipients(string r, Action<MailAddress> action)
        {
            var addresses = ConvertToAddresses(r);

            foreach (var mailAddress in addresses)
            {
                action(mailAddress);
            }
        }

        private MailAddress[] ConvertToAddresses(string addresses)
        {
            if (string.IsNullOrWhiteSpace(addresses))
                return null;

            var addrs = addresses.Split(',', ';');//TODO: setting?
            return addrs.Select(x => new MailAddress(x)).ToArray();
        }

        private static SmtpClient GetClient()
        {
            SmtpClient client = new SmtpClient();
            return client;
        }
    }
}
