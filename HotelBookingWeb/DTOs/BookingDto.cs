namespace HotelBookingWeb.DTOs
{
    public class BookingDto
    {
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}