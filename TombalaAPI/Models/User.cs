using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace TombalaAPI.Models
{
    public class User
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        [BsonElement("discordId")]
        public string DiscordId { get; set; }

        public bool IsAdmin { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

        public static void CreateIndexes(IMongoCollection<User> collection)
        {
            var indexKeysDefinition = Builders<User>.IndexKeys.Combine(
                Builders<User>.IndexKeys.Ascending(u => u.Name),
                Builders<User>.IndexKeys.Ascending(u => u.DiscordId)
            );

            var indexModel = new CreateIndexModel<User>(indexKeysDefinition, new CreateIndexOptions { Unique = true });

            collection.Indexes.CreateOne(indexModel);
        }
    }
}
