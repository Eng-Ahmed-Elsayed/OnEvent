using Models.Enums;
using Models.Interfaces;

namespace Models.Models
{
    public class Notification : Communication
    {
        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;
    }
}
