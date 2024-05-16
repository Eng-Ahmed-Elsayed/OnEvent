using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.Models
{
    public class Guest
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public MealPreferences MealPreference { get; set; } = MealPreferences.None;

        public Guid EventId { get; set; }
        public Event? Event { get; set; }
        public RSVP? RSVP { get; set; }
        public Guid? InvitationId { get; set; }
        public Invitation? Invitation { get; set; }
        public string? UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
