using Microsoft.AspNetCore.Identity;

namespace Models.Models
{
    public class User : IdentityUser
    {
        public ICollection<Event>? Events { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<EmailModel>? EmailModels { get; set; }
    }
}
