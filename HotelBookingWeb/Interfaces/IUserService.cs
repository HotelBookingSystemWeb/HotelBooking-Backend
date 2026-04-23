using HotelBookingWeb.DTOs;
using HotelBookingWeb.Models;

namespace HotelBookingWeb.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(int id, User updatedUser);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ToggleUserStatusAsync(int id);
        Task<object> GetDashboardSummaryAsync();
        Task<object> GetRecentBookingsAsync();
        Task<IEnumerable<Promotion>> GetAllPromotionsAsync();
        Task<Promotion?> GetPromotionByIdAsync(int id);
        Task<Promotion> CreatePromotionAsync(PromotionDto dto);
        Task<Promotion?> UpdatePromotionAsync(int id, PromotionDto dto);
        Task<bool> DeletePromotionAsync(int id);
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
    }
}