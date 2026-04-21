using HotelBookingWeb.DTOs;
using HotelBookingWeb.Models;

namespace HotelBookingWeb.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<Hotel>> GetAllAsync(string? location);
        Task<Hotel?> GetByIdAsync(int id);
        Task<Hotel> CreateAsync(HotelDto dto);
        Task<Hotel?> UpdateAsync(int id, HotelDto dto);
        Task<bool> DeleteAsync(int id);
    }
}