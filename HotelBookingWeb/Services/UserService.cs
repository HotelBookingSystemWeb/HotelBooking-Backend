using HotelBookingWeb.Data;
using HotelBookingWeb.DTOs;
using HotelBookingWeb.Interfaces;
using HotelBookingWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingWeb.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                Console.WriteLine("[UserService] Fetching all users...");
                return await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while fetching users: {ex.Message}");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                Console.WriteLine($"[UserService] Fetching user by Id: {id}");
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while fetching user by id: {ex.Message}");
                throw;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                Console.WriteLine($"[UserService] Fetching user by Email: {email}");
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while fetching user by email: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(int id, User updatedUser)
        {
            try
            {
                Console.WriteLine($"[UserService] Updating user with Id: {id}");

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (existingUser == null)
                {
                    Console.WriteLine("[UserService] User not found for update.");
                    return false;
                }

                existingUser.Name = updatedUser.Name;
                existingUser.PhoneNumber = updatedUser.PhoneNumber;
                existingUser.IsActive = updatedUser.IsActive;

                if (!string.IsNullOrWhiteSpace(updatedUser.Email))
                {
                    existingUser.Email = updatedUser.Email;
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("[UserService] User updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while updating user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                Console.WriteLine($"[UserService] Deleting user with Id: {id}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    Console.WriteLine("[UserService] User not found for delete.");
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                Console.WriteLine("[UserService] User deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while deleting user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            try
            {
                Console.WriteLine($"[UserService] Toggling user status for Id: {id}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    Console.WriteLine("[UserService] User not found for status toggle.");
                    return false;
                }

                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();

                Console.WriteLine($"[UserService] User status changed to: {user.IsActive}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while toggling user status: {ex.Message}");
                throw;
            }
        }

        public async Task<object> GetDashboardSummaryAsync()
        {
            try
            {
                Console.WriteLine("[UserService] Fetching dashboard summary...");

                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
                var totalHotels = await _context.Hotels.CountAsync();
                var totalRooms = await _context.Rooms.CountAsync();
                var totalBookings = await _context.Bookings.CountAsync();
                var activePromotions = await _context.Promotions.CountAsync(p => p.IsActive);

                var today = DateTime.UtcNow.Date;
                var todayBookings = await _context.Bookings
                    .CountAsync(b => b.CreatedAt.Date == today);

                var result = new
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    TotalHotels = totalHotels,
                    TotalRooms = totalRooms,
                    TotalBookings = totalBookings,
                    TodayBookings = todayBookings,
                    ActivePromotions = activePromotions
                };

                Console.WriteLine("[UserService] Dashboard summary fetched successfully.");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while fetching dashboard summary: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Promotion>> GetAllPromotionsAsync()
        {
            try
            {
                Console.WriteLine("[UserService] Fetching all promotions...");
                return await _context.Promotions
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while fetching promotions: {ex.Message}");
                throw;
            }
        }

        public async Task<Promotion?> GetPromotionByIdAsync(int id)
        {
            try
            {
                Console.WriteLine($"[UserService] Fetching promotion by Id: {id}");
                return await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while fetching promotion by id: {ex.Message}");
                throw;
            }
        }

        public async Task<Promotion> CreatePromotionAsync(PromotionDto dto)
        {
            try
            {
                Console.WriteLine("[UserService] Creating promotion...");

                if (dto.EndDate < dto.StartDate)
                {
                    throw new Exception("End date cannot be earlier than start date.");
                }

                var promotion = new Promotion
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    DiscountPercentage = dto.DiscountPercentage,
                    PromoCode = dto.PromoCode,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Promotions.Add(promotion);
                await _context.SaveChangesAsync();

                Console.WriteLine("[UserService] Promotion created successfully.");
                return promotion;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while creating promotion: {ex.Message}");
                throw;
            }
        }

        public async Task<Promotion?> UpdatePromotionAsync(int id, PromotionDto dto)
        {
            try
            {
                Console.WriteLine($"[UserService] Updating promotion Id: {id}");

                var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id);
                if (promotion == null)
                {
                    Console.WriteLine("[UserService] Promotion not found for update.");
                    return null;
                }

                if (dto.EndDate < dto.StartDate)
                {
                    throw new Exception("End date cannot be earlier than start date.");
                }

                promotion.Title = dto.Title;
                promotion.Description = dto.Description;
                promotion.DiscountPercentage = dto.DiscountPercentage;
                promotion.PromoCode = dto.PromoCode;
                promotion.StartDate = dto.StartDate;
                promotion.EndDate = dto.EndDate;
                promotion.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                Console.WriteLine("[UserService] Promotion updated successfully.");
                return promotion;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while updating promotion: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            try
            {
                Console.WriteLine($"[UserService] Deleting promotion Id: {id}");

                var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id);
                if (promotion == null)
                {
                    Console.WriteLine("[UserService] Promotion not found for delete.");
                    return false;
                }

                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();

                Console.WriteLine("[UserService] Promotion deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while deleting promotion: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            try
            {
                Console.WriteLine("[UserService] Fetching active promotions...");
                var now = DateTime.UtcNow;

                return await _context.Promotions
                    .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error while fetching active promotions: {ex.Message}");
                throw;
            }
        }
    }
}