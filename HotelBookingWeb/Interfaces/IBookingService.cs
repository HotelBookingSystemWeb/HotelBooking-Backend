using HotelBookingWeb.DTOs;
using HotelBookingWeb.Models;

namespace HotelBookingWeb.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> CreateBooking(int userId, BookingDto dto);
        Task<IEnumerable<Booking>> GetUserBookings(int userId);
        Task<bool> CancelBooking(int id, int userId);
        Task<Booking> UpdateStatus(int id, string status);
    }
}