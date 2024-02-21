using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.Models
{
    public class EmailCraft : Communication
    {
        [Required]
        [EmailAddress]
        public string EmailForReceiver { get; set; }
        public User? User { get; set; }
    }
}
