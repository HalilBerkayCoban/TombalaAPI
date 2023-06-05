using MongoDB.Bson.Serialization.Attributes;

namespace TombalaAPI.Models
{
    public class Game
    {
        [BsonId]
        public string Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }

        [BsonElement("users")]
        public List<User> Participants { get; set; }

        [BsonElement("gameCards")]
        public List<GameCard> GameCards { get; set; }

        public bool Active { get; set; }

        public List<int> DrawnNumbers { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

        public Game()
        {
            Participants = new List<User>();
            GameCards = new List<GameCard>();
            DrawnNumbers = new List<int>();
        }
    }
}
