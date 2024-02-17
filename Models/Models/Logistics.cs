using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.Models
{
    public class Logistics
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public List<EquipmentOptions>? EquipmentNeeded { get; set; }
        [StringLength(400, MinimumLength = 5)]
        public string? CateringDetails { get; set; }
        [StringLength(400, MinimumLength = 5)]
        public string? TransportationArrangements { get; set; }
    }
}
