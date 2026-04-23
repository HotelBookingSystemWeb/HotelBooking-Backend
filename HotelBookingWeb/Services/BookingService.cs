using HotelBookingWeb.Data;
using HotelBookingWeb.DTOs;
using HotelBookingWeb.Helpers;
using HotelBookingWeb.Interfaces;
using HotelBookingWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingWeb.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public BookingService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Booking> CreateBooking(int userId, BookingDto dto)
        {
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
                return null;

            if (dto.CheckInDate == default || dto.CheckOutDate == default)
                return null;

            if (dto.CheckInDate < DateTime.Today)
                return null;

            if (dto.CheckOutDate <= dto.CheckInDate)
                return null;

            var days = (dto.CheckOutDate - dto.CheckInDate).Days;

            var exists = await _context.Bookings.AnyAsync(b =>
                b.RoomId == dto.RoomId &&
                b.Status != "Cancelled" &&
                dto.CheckInDate < b.CheckOutDate &&
                dto.CheckOutDate > b.CheckInDate
            );

            if (exists)
                return null;

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

            // 🔥 SEND EMAIL AFTER BOOKING
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                var subject = "Booking Confirmed";

                var body = $@"
                    <h2>Booking Confirmed 🎉</h2>
                    <p><b>Booking Number:</b> {booking.BookingNumber}</p>
                    <p><b>Room ID:</b> {booking.RoomId}</p>
                    <p><b>Check-in:</b> {booking.CheckInDate:dd MMM yyyy}</p>
                    <p><b>Check-out:</b> {booking.CheckOutDate:dd MMM yyyy}</p>
                    <p><b>Total Amount:</b> ₹{booking.TotalAmount}</p>
                ";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

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
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

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

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }
    }
}