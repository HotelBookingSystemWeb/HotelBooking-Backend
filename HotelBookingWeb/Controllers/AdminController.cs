using HotelBookingWeb.DTOs;
using HotelBookingWeb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AdminController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                Console.WriteLine("[AdminController] GetDashboardSummary called.");
                var summary = await _userService.GetDashboardSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error in GetDashboardSummary: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching dashboard summary." });
            }
        }

        [HttpGet("promotions")]
        public async Task<IActionResult> GetAllPromotions()
        {
            try
            {
                Console.WriteLine("[AdminController] GetAllPromotions called.");
                var promotions = await _userService.GetAllPromotionsAsync();
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error in GetAllPromotions: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching promotions." });
            }
        }

        [AllowAnonymous]
        [HttpGet("promotions/active")]
        public async Task<IActionResult> GetActivePromotions()
        {
            try
            {
                Console.WriteLine("[AdminController] GetActivePromotions called.");
                var promotions = await _userService.GetActivePromotionsAsync();
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error in GetActivePromotions: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching active promotions." });
            }
        }

        [HttpGet("promotions/{id}")]
        public async Task<IActionResult> GetPromotionById(int id)
        {
            try
            {
                Console.WriteLine($"[AdminController] GetPromotionById called for Id: {id}");

                var promotion = await _userService.GetPromotionByIdAsync(id);
                if (promotion == null)
                {
                    return NotFound(new { message = "Promotion not found." });
                }

                return Ok(promotion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error in GetPromotionById: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching promotion." });
            }
        }

        [HttpPost("promotions")]
        public async Task<IActionResult> CreatePromotion([FromBody] PromotionDto dto)
        {
            try
            {
                Console.WriteLine("[AdminController] CreatePromotion called.");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var promotion = await _userService.CreatePromotionAsync(dto);
                return Ok(new
                {
                    message = "Promotion created successfully.",
                    data = promotion
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error in CreatePromotion: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("promotions/{id}")]
        public async Task<IActionResult> UpdatePromotion(int id, [FromBody] PromotionDto dto)
        {
            try
            {
                Console.WriteLine($"[AdminController] UpdatePromotion called for Id: {id}");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var promotion = await _userService.UpdatePromotionAsync(id, dto);
                if (promotion == null)
                {
                    return NotFound(new { message = "Promotion not found." });
                }

                return Ok(new
                {
                    message = "Promotion updated successfully.",
                    data = promotion
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error in UpdatePromotion: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("promotions/{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            try
            {
                Console.WriteLine($"[AdminController] DeletePromotion called for Id: {id}");

                var success = await _userService.DeletePromotionAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Promotion not found." });
                }

                return Ok(new { message = "Promotion deleted successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error in DeletePromotion: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting promotion." });
            }
        }

        [HttpPost("promotions/send-email")]
        public async Task<IActionResult> SendPromotionEmail([FromBody] PromotionEmailRequest request)
        {
            try
            {
                Console.WriteLine("[AdminController] SendPromotionEmail called.");

                if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body))
                {
                    return BadRequest(new { message = "Subject and body are required." });
                }

                var success = await _emailService.SendPromotionEmailToAllUsersAsync(request.Subject, request.Body);

                if (!success)
                {
                    return BadRequest(new { message = "Promotion email sending failed." });
                }

                return Ok(new { message = "Promotion email sent successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error in SendPromotionEmail: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while sending promotion email." });
            }
        }

        public class PromotionEmailRequest
        {
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }
    }
}