using Models.Models;

namespace Utility.Communication.MailTemplates
{
    public interface IMailTemplate
    {
        Task<string> Invitation(Event eventObj, Guid invitationId);
        Task<string> Reminder(Event eventObj, Guid invitationId);
        Task<string> Confirmation(Guest guest);

    }
}
