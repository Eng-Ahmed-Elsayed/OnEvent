using Models.Abstraction;
using Models.Enums;

namespace Models.Models
{
    public class Notification : Communication
    {
        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;
    }
}
