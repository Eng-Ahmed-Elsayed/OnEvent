using System.ComponentModel.DataAnnotations;
using Models.Enums;
using Models.Models;

namespace Models.DataTransferObjects
{
    public class InvitationDto
    {
        public Guid Id { get; set; }
        [Required]
        [EmailAddress]
        public string GuestEmail { get; set; }
        public InvitationStatus Status { get; set; } = InvitationStatus.Sent;
        public DateTime? ResponseDate { get; set; }

        public Guid? EventId { get; set; }
        public Event? Event { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
