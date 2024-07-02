using System.ComponentModel.DataAnnotations;
using Models.Enums;
using Models.Validators;

namespace Models.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 5)]
        // Brief description of the event and its purpose
        public string Brief { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 5)]
        // Brief overview of what will happen during the event
        public string Agenda { get; set; }
        [Required]
        [StringLength(800, MinimumLength = 5)]
        // Full description to the event
        public string Description { get; set; }
        [Required]
        public CategoryOptions Category { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DateRange("2000-01-01", "2099-12-31")]
        public DateTime Date { get; set; }
        [Required]
        [HourInDay(ErrorMessage = "Please enter a valid hour in a day.")]
        public TimeSpan Time { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 5)]
        public string Location { get; set; }
        [Required]
        public LocationType LocationType { get; set; }

        public string? ImgPath { get; set; }

        public string OrganizerId { get; set; }
        public User? Organizer { get; set; }
        public ICollection<Invitation>? Invitations { get; set; }
        public ICollection<Guest>? Guests { get; set; }
        public Logistics? Logistics { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
