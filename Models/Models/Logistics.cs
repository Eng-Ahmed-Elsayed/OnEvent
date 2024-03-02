using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.Models
{
    public class Logistics
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        // This field contains information about the equipment needed for the event.
        // It could include items such as audiovisual equipment, furniture, decorations, etc.
        [Display(Name = "Equipment Needed")]
        public List<EquipmentOption>? EquipmentNeeded { get; set; }
        // This field stores details about catering arrangements for the event.
        // It includes information about the type of food and beverages to be served,
        // dietary restrictions, menu options, etc.
        [Display(Name = "Catering Details")]
        public List<MenuOption>? CateringDetails { get; set; }
        // This field holds information regarding transportation arrangements for the event.
        // It may include details about transportation services provided for attendees, parking facilities,
        // shuttle services, etc. 
        [Display(Name = "Transportation Arrangements")]
        public List<Transportation>? TransportationArrangements { get; set; }
    }
}
