using Rain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Rain.Services
{
    public class RainService
    {
        public readonly IMongoCollection<RainModel> _rainfall_dataCollection;

        public RainService(IOptions<DatabaseSetting> rainDatabaseSetting)
        {
            var mongoClient = new MongoClient(rainDatabaseSetting.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(rainDatabaseSetting.Value.DatabaseName);
            _rainfall_dataCollection = mongoDatabase.GetCollection<RainModel>(rainDatabaseSetting.Value.CollectionName);
        }


        public async Task<List<RainModel>> GetAllEntries() =>
            await _rainfall_dataCollection.Find(_ => true).ToListAsync();

        public async Task<RainModel?> GetEntryById(string id) =>
          await _rainfall_dataCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task<List<RainModel>> GetEntryByDate(string date) =>
            await _rainfall_dataCollection.Find(x => x.date == date).ToListAsync();

        public async Task CreateEntry(RainModel newRain) =>
          await _rainfall_dataCollection.InsertOneAsync(newRain);

        public async Task UpdateEntry(string id, RainModel updatedRain) =>
          await _rainfall_dataCollection.ReplaceOneAsync(x => x.Id == id, updatedRain);

        public async Task RemoveEntry(string id) =>
          await _rainfall_dataCollection.DeleteOneAsync(x => x.Id == id);


    }
}
