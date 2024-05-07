using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Models.Enums;
using Models.Models;
using Models.Validators;

namespace Models.DataTransferObjects
{
    public class EventDto
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 5)]
        // Brief description of the event and its purpose
        public string Brief { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 5)]
        // Brief overview of what will happen during the event
        public string Agenda { get; set; }
        [Required]
        [StringLength(800, MinimumLength = 5)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DateRange("2000-01-01", "2099-12-31")]
        public DateTime Date { get; set; } = DateTime.Now;
        [Required]
        [HourInDay(ErrorMessage = "Please enter a valid hour in a day.")]
        public TimeSpan Time { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 5)]
        public string Location { get; set; }
        [Required]
        public LocationType LocationType { get; set; }

        public string? ImgPath { get; set; }
        [Display(Name = "Event Image")]
        [FileSize]
        public IFormFile? ImageFile { get; set; }


        public string? OrganizerId { get; set; }
        public User? Organizer { get; set; }
        public ICollection<Invitation>? Invitations { get; set; }
        public ICollection<GuestDto>? Guests { get; set; }
        public Logistics? Logistics { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
