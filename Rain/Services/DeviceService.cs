using Device.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Rain.Models;
using System;

namespace Device.Services
{
    public class DeviceService
    {
        private readonly IMongoCollection<DeviceModel> _rainfall_dataCollection;

        public DeviceService(IOptions<DatabaseSettings> databaseSettings)
        {
            var settings = databaseSettings?.Value;

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(databaseSettings), "Database settings are null.");
            }

            if (settings.RainDatabase == null)
            {
                throw new ArgumentNullException(nameof(settings.RainDatabase), "RainDatabase settings are null.");
            }

            // Debugging output
            //Console.WriteLine("RainDatabase ConnectionString: " + settings.RainDatabase.ConnectionString);
            //Console.WriteLine("RainDatabase DatabaseName: " + settings.RainDatabase.DatabaseName);
            //Console.WriteLine("RainDatabase CollectionName: " + settings.RainDatabase.CollectionName);

            var mongoClient = new MongoClient(settings.RainDatabase.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.RainDatabase.DatabaseName);
            _rainfall_dataCollection = mongoDatabase.GetCollection<DeviceModel>(settings.RainDatabase.CollectionName);
        }

        public async Task<List<DeviceModel>> GetAllEntries() =>
            await _rainfall_dataCollection.Find(_ => true).ToListAsync();

        public async Task<DeviceModel?> GetEntryById(string id) =>
            await _rainfall_dataCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<DeviceModel>> GetEntryByDate(string date) =>
            await _rainfall_dataCollection.Find(x => x.date == date).ToListAsync();

        public async Task CreateEntry(DeviceModel newRain) =>
            await _rainfall_dataCollection.InsertOneAsync(newRain);

        public async Task UpdateEntry(string id, DeviceModel updatedRain) =>
            await _rainfall_dataCollection.ReplaceOneAsync(x => x.Id == id, updatedRain);

        public async Task RemoveEntry(string id) =>
            await _rainfall_dataCollection.DeleteOneAsync(x => x.Id == id);
        public async Task<List<DeviceModel>> GetDevicesByDateTimeRange(string date, string startTime, string endTime)
        {
            var filter = Builders<DeviceModel>.Filter.And(
                Builders<DeviceModel>.Filter.Eq(device => device.date, date),
                Builders<DeviceModel>.Filter.Gte(device => device.time, startTime),
                Builders<DeviceModel>.Filter.Lte(device => device.time, endTime)
            );

            return await _rainfall_dataCollection.Find(filter).ToListAsync();
        }
    }
}
