using System.ComponentModel.DataAnnotations;

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
        public DateTime Date { get; set; }

        [Required]
        [StringLength(400, MinimumLength = 5)]
        public string Location { get; set; }

        public string OrganizerId { get; set; }
        public User? Organizer { get; set; }
        public ICollection<Invitation>? Invitations { get; set; }
        public ICollection<Guest>? Guests { get; set; }
        public ICollection<Logistics>? Logistics { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
