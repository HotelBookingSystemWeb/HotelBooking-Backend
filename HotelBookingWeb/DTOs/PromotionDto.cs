using System.ComponentModel.DataAnnotations;

namespace HotelBookingWeb.DTOs
{
    public class PromotionDto
    {
        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        [MaxLength(50)]
        public string? PromoCode { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}