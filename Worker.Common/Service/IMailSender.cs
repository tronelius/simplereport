namespace Worker.Common.Service
{
    public interface IMailSender
    {
        void Send(string mailSubject, string mailText, string to = null, string cc = null, string bcc = null, byte[] data = null);
    }
}