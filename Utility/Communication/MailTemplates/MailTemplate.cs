using Models.Models;
using Utility.Text;

namespace Utility.Communication.MailTemplates
{
    public class MailTemplate : IMailTemplate
    {
        public Task<string> Invitation(Event eventObj, Guid invitationId)
        {
            string message = $@"
                                Dear Guest,

                                I hope this email finds you well. I'm writing to personally invite you to {eventObj.Title}, which will be held on {eventObj.Date} at {eventObj.Time} at [Venue/Online Platform].

                                {eventObj.Title} is {eventObj.Brief}. We believe your presence would greatly enhance the experience for all attendees.

                                Here are some key details about the event:

                                Date: {eventObj.Date}
                                Time: {eventObj.Time}
                                Venue/Platform: [Location/Online Link]
                                Location: {eventObj.Location}
                                Agenda: {eventObj.Agenda}

                                Your expertise and insights would be invaluable to our discussions. Please let us know if you can attend by {eventObj.Date}.

                                You can confirm your attendance by visiting https://localhost:7132/guests/registration/{invitationId}.

                                If you have any questions or need further information, feel free to contact me directly at [OnEvent Help Link].

                                Thank you for considering our invitation. We hope to see you at {eventObj.Title}!

                                Best regards,

                                {StringGlobalization.ToTitleCase(eventObj.Organizer.FullName)}
                                OnEvent
                                ";
            return Task.FromResult(message);
        }

        public Task<string> Reminder(Event eventObj, Guid invitationId)
        {
            string message = $@"
                                Dear Dear Guest,

                                We hope this email finds you well. We wanted to send you a friendly reminder that [Event Name] is taking place tomorrow, [Date], at [Time] at [Venue/Online Platform].

                                We're thrilled about the opportunity to gather with such esteemed guests and delve into meaningful discussions surrounding [brief description of the event and its purpose].

                                Here are the key details once again:

                                Date: [Date]
                                Time: [Time]
                                Venue/Platform: [Location/Online Link]
                                Please make sure to mark your calendar and join us promptly at the scheduled time. Your presence will undoubtedly enrich our event, and we're looking forward to your contributions.

                                If you haven't already RSVP'd, there's still time to confirm your attendance by replying to this email or visiting https://localhost:7132/guests/registration/{invitationId}.

                                Should you have any questions or need further information, don't hesitate to reach out to us directly at [Your Contact Information].

                                Thank you again for considering our invitation. We're eager to see you at [Event Name] tomorrow!

                                Best regards,

                                [Your Name]
                                [Your Position/Organization]
                                [Your Contact Information]
                                ";
            return Task.FromResult(message);
        }

        public Task<string> Confirmation(Guest guest)
        {
            string message = $@"
                            Dear {guest.Name},

                            We are delighted to inform you that your registration for {guest?.Event?.Title} has been successfully received!

                            Here are the details of your registration:
                            
                            Guest: {guest.Name}
                            Meal Preference: {guest.MealPreference}
                            RSVP Status: {guest.RSVP.RSVPStatus}
                            Guest Code: {guest.Id}

                            Your presence at {guest?.Event?.Title} is greatly appreciated, and we are looking forward to your participation in our event discussions and activities.

                            Should you have any questions, change your info or require further information, please do not hesitate to reach out to us at https://localhost:7132/guests/{guest.Id}/info.

                            Thank you once again for registering, and we look forward to seeing you at {guest?.Event?.Title}!

                            Best regards,

                            {StringGlobalization.ToTitleCase(guest.Event.Organizer.FullName)}
                            OnEvent
                            ";
            return Task.FromResult(message);

        }

    }
}
