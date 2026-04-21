using HotelBookingWeb.Data;
using HotelBookingWeb.DTOs;
using HotelBookingWeb.Helpers;
using HotelBookingWeb.Interfaces;
using HotelBookingWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace HotelBookingWeb.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateBooking(int userId, BookingDto dto)
        {
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
                throw new Exception("Room not found");

            var days = (dto.CheckOutDate - dto.CheckInDate).Days;
            if (days <= 0)
                throw new Exception("Invalid dates");

            var booking = new Booking
            {
                BookingNumber = BookingNumberGenerator.Generate(),
                UserId = userId,
                RoomId = dto.RoomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalAmount = days * room.PricePerNight,
                Status = "Confirmed"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }

        public async Task<IEnumerable<Booking>> GetUserBookings(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> CancelBooking(int id, int userId)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null || booking.UserId != userId)
                return false;

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Booking> UpdateStatus(int id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return null;

            booking.Status = status;
            await _context.SaveChangesAsync();

            return booking;
        }
    }
}