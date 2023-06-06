using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TombalaAPI.Models;

namespace TombalaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;

        public UsersController(IOptions<DatabaseSettings> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(
            options.Value.DatabaseName);

            _users = mongoDatabase.GetCollection<User>(
            options.Value.UsersCollectionName);
        }

        [Authorize(AuthenticationSchemes = "Discord")]
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            var user = _users.Find(u => u.DiscordId == claims[0].Value).FirstOrDefault();

            if (user == null)
            {
                var newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    DiscordId = claims[0].Value,
                    Name = claims[1].Value,
                    IsAdmin = false,
                };

                _users.InsertOne(newUser);
            }

            return Ok(claims);
        }
    }
}
