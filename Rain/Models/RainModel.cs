using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Rain.Models
{
    public class RainModel
    {
        //[BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonIgnore]
        public  string? Id { get; set; }

        public  string date { get; set; } 

        public  double rainfall { get; set; }
    }
}