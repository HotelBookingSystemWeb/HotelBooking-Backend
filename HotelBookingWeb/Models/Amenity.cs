using System.ComponentModel.DataAnnotations;

namespace HotelBookingWeb.Models
{
    public class Amenity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}