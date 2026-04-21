using HotelBookingWeb.Data;
using HotelBookingWeb.DTOs;
using HotelBookingWeb.Interfaces;
using HotelBookingWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace HotelBookingWeb.Services
{
    public class HotelService : IHotelService
    {
        private readonly AppDbContext _context;

        public HotelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> GetAllAsync(string? location)
        {
            var query = _context.Hotels.AsQueryable();

            if (!string.IsNullOrEmpty(location))
                query = query.Where(h => h.Location.Contains(location));

            return await query.Include(h => h.Rooms).ToListAsync();
        }

        public async Task<Hotel?> GetByIdAsync(int id)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<Hotel> CreateAsync(HotelDto dto)
        {
            var hotel = new Hotel
            {
                Name = dto.Name,
                Location = dto.Location,
                Description = dto.Description
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return hotel;
        }

        public async Task<Hotel?> UpdateAsync(int id, HotelDto dto)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return null;

            hotel.Name = dto.Name;
            hotel.Location = dto.Location;
            hotel.Description = dto.Description;

            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}