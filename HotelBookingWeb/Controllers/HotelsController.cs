using HotelBookingWeb.DTOs;
using HotelBookingWeb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _service;

        public HotelsController(IHotelService service)
        {
            _service = service;
        }

        // PUBLIC
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? location)
        {
            var hotels = await _service.GetAllAsync(location);
            return Ok(hotels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var hotel = await _service.GetByIdAsync(id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }

        // ADMIN
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(HotelDto dto)
        {
            var hotel = await _service.CreateAsync(dto);
            return Ok(hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, HotelDto dto)
        {
            var hotel = await _service.UpdateAsync(id, dto);
            if (hotel == null) return NotFound();
            return Ok(hotel);
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