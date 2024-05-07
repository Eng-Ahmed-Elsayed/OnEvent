using Models.Models;

namespace Utility.Communication.Mail
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailCraft emailCraft);
    }
}
