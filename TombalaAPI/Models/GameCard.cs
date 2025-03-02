using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TombalaAPI.Models
{
    public class GameCard
    {
        [Key]
        public string Id { get; set; }

        public string GameId { get; set; }

        public User User { get; set; }
        
        public string UserId { get; set; }

        public List<int> Numbers { get; set; }

        public List<int> MarkedNumbers { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public GameCard()
        {
            Numbers = new List<int>();
            MarkedNumbers = new List<int>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
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
                var number = allNumbers[randomIndex];
                Numbers.Add(number);
                allNumbers.RemoveAt(randomIndex);
            }
        }
    }
}
