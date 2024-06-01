using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rain.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("email")]
        public string email { get; set; }

        [BsonElement("password")]
        public string password { get; set; }
    }
}
