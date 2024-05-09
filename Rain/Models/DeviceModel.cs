using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Libmongocrypt;
using System.Text.Json.Serialization;

namespace Device.Models
{
    public class DeviceModel : Controller
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonIgnore]
        public string? Id { get; set; }

        public double temperature_C { get; set; }

        public double humidity { get; set; }
        public double pressure_hPa { get; set; }
        public string BME280 { get; set; }
        public double wind { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string value { get; set; }
    }
}
