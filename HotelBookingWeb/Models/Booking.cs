using System.ComponentModel.DataAnnotations;

namespace HotelBookingWeb.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string BookingNumber { get; set; }

        public int UserId { get; set; }

        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Confirmed";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}