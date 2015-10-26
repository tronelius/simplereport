using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Worker.Common.Model;

namespace Worker.Common.Service
{
    public class MailSender : IMailSender
    {
        private readonly IApplicationSettings _applicationSettings;

        public MailSender(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public void Send(string mailSubject, string mailText, string to = null, string cc = null, string bcc = null, byte[] data = null, string fileName = null)
        {
            var client = GetClient();

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_applicationSettings.EmailFromAddress);

            AddRecipients(to, mail.To.Add);
            AddRecipients(cc, mail.CC.Add);
            AddRecipients(bcc, mail.Bcc.Add);

            mail.Subject = mailSubject;
            mail.Body = mailText;

            if (data != null && data.Length > 0)
            {
                using (MemoryStream memStream = new MemoryStream(data))
                {
                    mail.Attachments.Add(new Attachment(memStream, fileName));
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

            if (addresses == null)
                return;

            foreach (var mailAddress in addresses)
            {
                action(mailAddress);
            }
        }

        private MailAddress[] ConvertToAddresses(string addresses)
        {
            if (string.IsNullOrWhiteSpace(addresses))
                return null;

            var addrs = addresses.Split(',', ';');
            return addrs.Select(x => new MailAddress(x)).ToArray();
        }

        private static SmtpClient GetClient()
        {
            SmtpClient client = new SmtpClient();
            return client;
        }
    }
}
