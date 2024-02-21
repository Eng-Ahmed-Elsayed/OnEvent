using System.ComponentModel.DataAnnotations;
using Models.Enums;
using Models.Models;

namespace Models.DataTransferObjects
{
    public class GuestDto
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public MealPreferences MealPreference { get; set; } = MealPreferences.None;

        public Guid? EventId { get; set; }
        public EventDto? Event { get; set; }
        public RSVP? RSVP { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
