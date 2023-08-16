using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace HT.WaterAlerts.Common.Email
{
    public class SmtpClient : ISmtpClient
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<SmtpClient> _logger;
        public SmtpClient(IOptions<SmtpSettings> smtpSettings, ILogger<SmtpClient> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;

        }
        public void Send(string to, string subject, string body)
        {
            try
            {
                MailMessage mailMessage = new MailMessage(_smtpSettings.FromAddress, to, subject, body);
                mailMessage.IsBodyHtml = true;

                using (var client = new System.Net.Mail.SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    client.Send(mailMessage);
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogError("Mail sending failed, subject: " + subject + ", email: " + to + ", error: " + ex.Message, ex);
                throw;
            }
        }
    }
}