using MongoDB.Bson.Serialization.Attributes;

namespace TombalaAPI.Models
{
    public class GameCard
    {
        [BsonId]
        public string Id { get; set; }

        public string GameId { get; set; }

        public User User { get; set; }

        public List<int> Numbers { get; set; }

        public List<int> MarkedNumbers { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

        public GameCard()
        {
            Numbers = new List<int>();
            MarkedNumbers = new List<int>();
            GenerateNumbers();
        }

        public void GenerateNumbers()
        {
            if (Numbers.Count > 0)
                return;

            var allNumbers = new List<int>();
            for (int i = 1; i <= 90; i++)
            {
                allNumbers.Add(i);
            }

            var random = new Random();
            for (int i = 0; i < 15; i++)
            {
                var randomIndex = random.Next(allNumbers.Count);
                Numbers.Add(randomIndex);
                allNumbers.Remove(randomIndex);
            }
        }
    }
}
