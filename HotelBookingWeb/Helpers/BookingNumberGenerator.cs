namespace HotelBookingWeb.Helpers
{
    public static class BookingNumberGenerator
    {
        public static string Generate()
        {
            return "BOOK-" + DateTime.UtcNow.Ticks;
        }
    }
}