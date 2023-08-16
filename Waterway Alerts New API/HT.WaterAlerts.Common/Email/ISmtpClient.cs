namespace HT.WaterAlerts.Common.Email
{
    public interface ISmtpClient
    {
        void Send(string to, string subject, string body);
    }
}
