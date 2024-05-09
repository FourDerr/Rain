using Device.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Rain.Models;

namespace Device.Services
{
    public class DeviceService
    {
        public readonly IMongoCollection<DeviceModel> _rainfall_dataCollection;

        public DeviceService(IOptions<DatabaseSetting> DeviceDatabaseSetting)
        {
            var mongoClient = new MongoClient(DeviceDatabaseSetting.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(DeviceDatabaseSetting.Value.DatabaseName);
            _rainfall_dataCollection = mongoDatabase.GetCollection<DeviceModel>(DeviceDatabaseSetting.Value.CollectionName);
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

        //public async Task<DeviceModel> GetDeviceById(string deviceId)
        //{
        //    return await _rainfall_dataCollection.Find(d => d.Id == deviceId).FirstOrDefaultAsync();
        //}
    }
}