using MongoDB.Bson.Serialization.Attributes;

namespace Rain.Models
{
    public class RainModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public required string? Id { get; set; }

        public required string date { get; set; } 

        public required double rainfall { get; set; }
    }
}