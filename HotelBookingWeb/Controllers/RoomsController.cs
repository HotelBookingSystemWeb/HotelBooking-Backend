using HotelBookingWeb.DTOs;
using HotelBookingWeb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _service;

        public RoomsController(IRoomService service)
        {
            _service = service;
        }

        // PUBLIC FILTER API 🔥
        [HttpGet]
        public async Task<IActionResult> GetRooms(
            [FromQuery] int? hotelId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var rooms = await _service.GetRoomsAsync(hotelId, minPrice, maxPrice);
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var room = await _service.GetByIdAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        // ADMIN
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(RoomDto dto)
        {
            var room = await _service.CreateAsync(dto);
            return Ok(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RoomDto dto)
        {
            var room = await _service.UpdateAsync(id, dto);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok("Deleted Successfully");
        }
    }
}