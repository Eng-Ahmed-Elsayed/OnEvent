using System.ComponentModel.DataAnnotations;
using Models.Models;

namespace Models.DataTransferObjects
{
    public class EventDto
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
        public string? ImgPath { get; set; }


        public string? OrganizerId { get; set; }
        public User? Organizer { get; set; }
        public ICollection<Invitation>? Invitations { get; set; }
        public ICollection<GuestDto>? Guests { get; set; }
        public ICollection<Logistics>? Logistics { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
