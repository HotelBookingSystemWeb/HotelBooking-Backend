using HotelBookingWeb.Interfaces;
using HotelBookingWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                Console.WriteLine("[UsersController] GetAllUsers called.");
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsersController] Error in GetAllUsers: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching users." });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                Console.WriteLine("[UsersController] GetMyProfile called.");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var userId = int.Parse(userIdClaim);
                var user = await _userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsersController] Error in GetMyProfile: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching profile." });
            }
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] User updatedUser)
        {
            try
            {
                Console.WriteLine("[UsersController] UpdateMyProfile called.");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var userId = int.Parse(userIdClaim);

                var existingUser = await _userService.GetUserByIdAsync(userId);
                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                updatedUser.Role = existingUser.Role;
                updatedUser.PasswordHash = existingUser.PasswordHash;

                var success = await _userService.UpdateUserAsync(userId, updatedUser);
                if (!success)
                {
                    return BadRequest(new { message = "Profile update failed." });
                }

                return Ok(new { message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsersController] Error in UpdateMyProfile: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating profile." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                Console.WriteLine($"[UsersController] DeleteUser called for Id: {id}");

                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsersController] Error in DeleteUser: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting user." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            try
            {
                Console.WriteLine($"[UsersController] ToggleUserStatus called for Id: {id}");

                var success = await _userService.ToggleUserStatusAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(new { message = "User status updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsersController] Error in ToggleUserStatus: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating user status." });
            }
        }
    }
}