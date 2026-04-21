namespace HotelBookingWeb.Models
{
    public class Room
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}