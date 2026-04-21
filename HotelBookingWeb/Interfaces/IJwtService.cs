using HotelBookingWeb.Models;

namespace HotelBookingWeb.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}