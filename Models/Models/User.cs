using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Models.Models
{
    public class User : IdentityUser
    {
        public string? ProfileImgPath { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public ICollection<Event>? Events { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<EmailCraft>? EmailModels { get; set; }
    }
}
