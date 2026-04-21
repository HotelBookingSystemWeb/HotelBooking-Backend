using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        [Required]
        public string Role { get; set; } = "User"; // Admin / User

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Navigation Property (Future Use)
        public ICollection<Booking>? Bookings { get; set; }
    }
}