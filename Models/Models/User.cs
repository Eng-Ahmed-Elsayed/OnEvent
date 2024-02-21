using Microsoft.AspNetCore.Identity;

namespace Models.Models
{
    public class User : IdentityUser
    {
        public string? ProfileImgPath { get; set; }

        public ICollection<Event>? Events { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<EmailCraft>? EmailModels { get; set; }
    }
}
