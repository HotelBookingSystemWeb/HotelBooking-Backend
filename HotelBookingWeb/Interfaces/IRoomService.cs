using HotelBookingWeb.DTOs;
using HotelBookingWeb.Models;

namespace HotelBookingWeb.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<Room>> GetRoomsAsync(int? hotelId, decimal? minPrice, decimal? maxPrice);
        Task<Room?> GetByIdAsync(int id);
        Task<Room> CreateAsync(RoomDto dto);
        Task<Room?> UpdateAsync(int id, RoomDto dto);
        Task<bool> DeleteAsync(int id);
    }
}