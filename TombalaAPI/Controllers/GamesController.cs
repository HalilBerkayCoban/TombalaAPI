using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using TombalaAPI.Hubs;
using TombalaAPI.Models;

namespace TombalaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<GameHub> _hubContext;
        private static List<int> availableNumbers = Enumerable.Range(1, 90).ToList();
        private static Random random = new Random();

        public GamesController(ApplicationDbContext context, IHubContext<GameHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Game>> CreateGame([FromQuery] string name)
        {
            var game = new Game
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Active = false
            };
            
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("NewGame");

            return Ok(game);
        }

        [HttpPost("{id}/status")]
        public async Task<ActionResult<Game>> ChangeGameStatus(string id, [FromQuery] string status)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return NotFound();

            game.Active = status == "start";
            game.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("GameStatusChanged");

            return Ok(game);
        }

        [HttpPost("{id}/numbers")]
        public async Task<ActionResult<int>> DrawNumber(string id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return NotFound();

            int randomIndex = random.Next(availableNumbers.Count);
            int drawnNumber = availableNumbers[randomIndex];

            availableNumbers.RemoveAt(randomIndex);

            game.DrawnNumbers.Add(drawnNumber);
            game.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("NewDraw", drawnNumber);

            return Ok(drawnNumber);
        }

        [HttpPut("{gameId}/cards/{cardId}/numbers/{number}")]
        public async Task<ActionResult> MarkNumber(string gameId, string cardId, int number, [FromQuery] bool mark)
        {
            if (mark != true)
                return BadRequest();

            var game = await _context.Games
                .Include(g => g.DrawnNumbers)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                return NotFound();

            if (game.DrawnNumbers[game.DrawnNumbers.Count - 1] != number)
                return BadRequest();

            var card = await _context.GameCards.FindAsync(cardId);
            if (card == null)
                return NotFound();

            card.MarkedNumbers.Add(number);
            card.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("NewUserAction");

            return Ok();
        }

        [HttpPut("{gameId}/cards/{cardId}")]
        public async Task<ActionResult> RequestANewCard(string gameId, string cardId, [FromQuery] string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
            if (user == null)
                return NotFound("User not found");

            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
                return NotFound("Game not found");

            var card = await _context.GameCards.FindAsync(cardId);
            if (card == null)
            {
                card = new GameCard 
                { 
                    Id = cardId, 
                    GameId = gameId, 
                    User = user,
                    UserId = user.Id
                };
                _context.GameCards.Add(card);
            }
            else
            {
                card.User = user;
                card.UserId = user.Id;
                card.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("NewCard");

            return Ok();
        }

        [HttpGet("games")]
        public async Task<ActionResult<List<Game>>> GetAllGames()
        {
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.IsAdmin == true);

            if (adminUser == null)
                return Ok(await _context.Games.Where(g => g.Active == true).ToListAsync());
            
            return Ok(await _context.Games.ToListAsync());
        }

        [HttpPost("{id}/participants")]
        public async Task<IActionResult> AddParticipant(string id, [FromQuery] string username)
        {
            var game = await _context.Games
                .Include(g => g.Participants)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
            if (user == null)
                return NotFound("User not found");

            game.Participants.Add(user);
            game.UpdatedAt = DateTime.UtcNow;

            var card = new GameCard 
            { 
                Id = Guid.NewGuid().ToString(), 
                GameId = id, 
                User = user,
                UserId = user.Id
            };
            
            _context.GameCards.Add(card);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("NewParticipant", id);

            return Ok(game);
        }
    }
}
