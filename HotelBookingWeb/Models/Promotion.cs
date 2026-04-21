using System.ComponentModel.DataAnnotations;

namespace HotelBookingWeb.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        [MaxLength(50)]
        public string? PromoCode { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}