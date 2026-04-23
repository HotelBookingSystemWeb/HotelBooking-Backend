using HotelBookingWeb.Data;
using HotelBookingWeb.DTOs;
using HotelBookingWeb.Interfaces;
using HotelBookingWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace HotelBookingWeb.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetRoomsAsync(int? hotelId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Rooms.AsQueryable();

            if (hotelId.HasValue)
                query = query.Where(r => r.HotelId == hotelId);

            if (minPrice.HasValue)
                query = query.Where(r => r.PricePerNight >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(r => r.PricePerNight <= maxPrice);

            return await query.Include(r => r.Hotel).ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Room> CreateAsync(RoomDto dto)
        {
            var room = new Room
            {
                RoomNumber = dto.RoomNumber,
                PricePerNight = dto.Price,
                Capacity = dto.Capacity,
                HotelId = dto.HotelId
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }

        public async Task<Room?> UpdateAsync(int id, RoomDto dto)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return null;

            room.RoomNumber = dto.RoomNumber;
            room.PricePerNight = dto.Price;
            room.Capacity = dto.Capacity;

            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut)
        {
            return await _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .Where(r => !_context.Bookings.Any(b =>
                    b.RoomId == r.Id &&
                    b.Status != "Cancelled" &&
                    checkIn < b.CheckOutDate &&
                    checkOut > b.CheckInDate
                ))
                .ToListAsync();
        }
    }
}