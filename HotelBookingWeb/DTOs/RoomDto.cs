namespace HotelBookingWeb.DTOs
{
    public class RoomDto
    {
        public string RoomNumber { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int HotelId { get; set; }
    }
}