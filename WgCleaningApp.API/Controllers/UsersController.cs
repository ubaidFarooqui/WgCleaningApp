using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WgCleaningApp.Infrastructure.Persistence;
using WgCleaningApp.Infrastructure.Services;

namespace WgCleaningApp.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Name, u.Email, u.Role })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            return Ok(new { name = user.Name, email = user.Email, role = user.Role });


        }

        [Authorize]
        [HttpPost("device-token")]
        public async Task<IActionResult> SaveDeviceToken([FromBody] SaveDeviceTokenRequest request)
        {
            var userId = User.GetUserId(); // however you read from JWT;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.UpdateDeviceToken(request.DeviceToken);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class SaveDeviceTokenRequest
        {
            public string DeviceToken { get; set; } = string.Empty;
        }

    }
}
