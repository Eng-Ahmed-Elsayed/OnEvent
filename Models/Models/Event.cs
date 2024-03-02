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
        [StringLength(800, MinimumLength = 5)]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DateRange("2000-01-01", "2099-12-31")]
        public DateTime Date { get; set; }
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
