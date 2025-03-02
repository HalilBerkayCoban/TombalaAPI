using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TombalaAPI.Models
{
    public class Game
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<User> Participants { get; set; }

        public List<GameCard> GameCards { get; set; }

        public bool Active { get; set; }

        public List<int> DrawnNumbers { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Game()
        {
            Participants = new List<User>();
            GameCards = new List<GameCard>();
            DrawnNumbers = new List<int>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
