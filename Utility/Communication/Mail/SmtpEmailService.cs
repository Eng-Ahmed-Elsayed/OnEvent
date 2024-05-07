using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Models.Models;

namespace Utility.Communication.Mail
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(SmtpSettings smtpSettings, ILogger<SmtpEmailService> logger)
        {
            _smtpSettings = smtpSettings;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailCraft emailCraft)
        {
            var emailMessage = CreateEmailMessage(emailCraft);
            await SendAsync(emailMessage);
        }

        private MimeMessage CreateEmailMessage(EmailCraft emailCraft)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("OnEvent", _smtpSettings.SmtpUsername));
            emailMessage.To.Add(new MailboxAddress("Guest", emailCraft.EmailForReceiver));
            emailMessage.Subject = emailCraft.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = emailCraft.Message };

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_smtpSettings.SmtpUsername, _smtpSettings.SmtpPassword);

                    await client.SendAsync(mailMessage);
                    _logger.LogInformation("Send email message successfully!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
