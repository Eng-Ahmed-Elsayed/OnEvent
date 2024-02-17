using System.ComponentModel.DataAnnotations;

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
        [StringLength(200, MinimumLength = 5)]
        public string MealPreference { get; set; }

        public Guid EventId { get; set; }
        public Event? Event { get; set; }
        public RSVP RSVP { get; set; }


        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
