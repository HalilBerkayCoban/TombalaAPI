using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using TombalaAPI.Models;

namespace TombalaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IMongoCollection<Game> _games;
        private readonly IMongoCollection<GameCard> _cards;
        private readonly IMongoCollection<User> _users;
        private readonly IHubContext _hubContext;
        private static List<int> availableNumbers = Enumerable.Range(1, 90).ToList();
        private static Random random = new Random();

        public GamesController(IOptions<DatabaseSettings> options, IHubContext hubContext)
        {
            var mongoClient = new MongoClient(
            options.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(
            options.Value.DatabaseName);

            _games = mongoDatabase.GetCollection<Game>(
            options.Value.GamesCollectionName);

            _cards = mongoDatabase.GetCollection<GameCard>(
            options.Value.GameCardsCollectionName);

            _users = mongoDatabase.GetCollection<User>(
            options.Value.UsersCollectionName);

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
            _games.InsertOne(game);

            await _hubContext.Clients.All.SendAsync("NewGame");

            return Ok(game);
        }

        [HttpPost("{id}/status")]
        public async Task<ActionResult<Game>> ChangeGameStatus(string id, [FromQuery] string status)
        {
            var game = _games.Find(g => g.Id == id).FirstOrDefault();

            if (game == null)
                return NotFound();

            game.Active = status == "start";

            await _games.ReplaceOneAsync(g => g.Id == id, game);

            await _hubContext.Clients.All.SendAsync("GameStatusChanged");

            return Ok(game);
        }

        [HttpPost("{id}/numbers")]
        public async Task<ActionResult<int>> DrawNumber(string id)
        {
            var game = _games.Find(g => g.Id == id).FirstOrDefault();

            if (game == null)
                return NotFound();

            int randomIndex = random.Next(availableNumbers.Count);
            int drawnNumber = availableNumbers[randomIndex];

            availableNumbers.RemoveAt(randomIndex);

            await _games.UpdateOneAsync(g => g.Id == id, Builders<Game>.Update.Push(g => g.DrawnNumbers, drawnNumber));

            await _hubContext.Clients.All.SendAsync("NewDraw", drawnNumber);

            return Ok(drawnNumber);
        }

        [HttpPut("{gameId}/cards/{cardId}/numbers/{number}")]
        public async Task<ActionResult> MarkNumber(string gameId, string cardId, int number, [FromQuery] bool mark)
        {
            if (mark != true)
                return BadRequest();

            var game = _games.Find(g => g.Id == gameId).FirstOrDefault();

            if (game == null)
                return NotFound();

            if (game.DrawnNumbers[game.DrawnNumbers.Count - 1] != number)
                return BadRequest();

            await _games.UpdateOneAsync(x => x.Id == gameId && x.GameCards.Any(card => card.Id == cardId), Builders<Game>.Update.AddToSet("GameCards.$.MarkedNumbers", number));
            await _cards.UpdateOneAsync(x => x.Id == cardId, Builders<GameCard>.Update.Push(g => g.MarkedNumbers, number));

            await _hubContext.Clients.All.SendAsync("NewUserAction");

            return Ok();
        }

        [HttpPut("{gameId}/cards/{cardId}")]
        public async Task<ActionResult> RequestANewCard(string gameId, string cardId, [FromQuery] string username)
        {
            var user = _users.Find(x => x.Name == username).FirstOrDefault();
            var card = new GameCard { Id = cardId, GameId = gameId, User = user };

            await _games.UpdateOneAsync(x => x.Id == gameId && x.GameCards.Any(card => card.Id == cardId), Builders<Game>.Update.Set("GameCards.$", card));

            await _hubContext.Clients.All.SendAsync("NewCard");

            return Ok();
        }

        [HttpGet("games")]
        public ActionResult<List<Game>> GetAllGames()
        {
            var games = _games.Find(_ => true).ToList();
            var user = _users.Find(u => u.IsAdmin == true).FirstOrDefault();

            if (user == null)
                games = _games.Find(g => g.Active == true).ToList();

            return Ok(games);
        }


        [HttpPost("{id}/participants")]
        public async Task<IActionResult> AddParticipant(string id, [FromQuery] string username)
        {
            var game = await _games.Find(g => g.Id == id).FirstOrDefaultAsync();

            if (game == null)
                return NotFound();

            var user = _users.Find(x => x.Name == username).FirstOrDefault();

            await _games.UpdateOneAsync(g => g.Id == id, Builders<Game>.Update.Push(g => g.Participants, user));

            var card = new GameCard { Id = Guid.NewGuid().ToString(), GameId = id, User = user };
            await _games.UpdateOneAsync(x => x.Id == id, Builders<Game>.Update.Push(g => g.GameCards, card));

            await _hubContext.Clients.All.SendAsync("NewParticipant", id);

            return Ok(game);
        }
    }
}
