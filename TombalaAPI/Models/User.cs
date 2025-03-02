using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TombalaAPI.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Column("discord_id")]
        public string DiscordId { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public User()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
