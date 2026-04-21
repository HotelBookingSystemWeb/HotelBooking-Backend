using HotelBookingWeb.DTOs;
using HotelBookingWeb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔐 ALL APIs REQUIRE LOGIN
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingsController(IBookingService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingDto dto)
        {
            var userId = GetUserId();
            var result = await _service.CreateBooking(userId, dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetUserId();
            var result = await _service.GetUserBookings(userId);
            return Ok(result);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            var result = await _service.CancelBooking(id, userId);
            return result ? Ok("Cancelled") : Unauthorized();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")] // 🔥 ONLY ADMIN
        public async Task<IActionResult> UpdateStatus(int id, BookingStatusDto dto)
        {
            var result = await _service.UpdateStatus(id, dto.Status);
            return result != null ? Ok(result) : NotFound();
        }
    }
}