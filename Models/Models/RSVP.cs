using Models.Enums;

namespace Models.Models
{
    public class RSVP
    {
        public Guid Id { get; set; }
        public Guid GuestId { get; set; }
        public Guid EventId { get; set; }
        /// <summary>
        /// RSVPStatus => Attending, NotAttending, Undecided (default)
        /// </summary>
        public RSVPStatus RSVPStatus { get; set; } = RSVPStatus.Undecided;
    }
}
