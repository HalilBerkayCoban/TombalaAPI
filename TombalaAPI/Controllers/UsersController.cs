using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TombalaAPI.Models;

namespace TombalaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(AuthenticationSchemes = "Discord")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.DiscordId == claims[0].Value);

            if (user == null)
            {
                var newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    DiscordId = claims[0].Value,
                    Name = claims[1].Value,
                    IsAdmin = false,
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }

            return Ok(claims);
        }
    }
}
