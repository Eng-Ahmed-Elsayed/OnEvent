using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.Models
{
    public class Invitation
    {
        public Guid Id { get; set; }
        [Required]
        [EmailAddress]
        public string GuestEmail { get; set; }
        public InvitationStatus Status { get; set; } = InvitationStatus.Sent;
        public DateTime? ResponseDate { get; set; }

        public Guid EventId { get; set; }
        public Event? Event { get; set; }
        public Guest? Guest { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
