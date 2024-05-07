using System.ComponentModel.DataAnnotations;

namespace Models.Abstraction
{
    public abstract class Communication
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(800, MinimumLength = 5)]
        public string Subject { get; set; }
        [Required]
        [StringLength(2000)]
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public Guid EventId { get; set; }
        public string UserId { get; set; }

    }
}
