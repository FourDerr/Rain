using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Rain.Models;

public class UserService
{
    private readonly IMongoCollection<UserModel> _userCollection;

    public UserService(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.RainWebDatabase.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.RainWebDatabase.DatabaseName);
        _userCollection = mongoDatabase.GetCollection<UserModel>(databaseSettings.Value.RainWebDatabase.CollectionName);
    }

    public async Task<List<UserModel>> GetAllUsers() =>
        await _userCollection.Find(_ => true).ToListAsync();

    public async Task<UserModel> GetUserById(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            return null; // or handle the error as appropriate
        }

        return await _userCollection.Find(x => x.Id == objectId.ToString()).FirstOrDefaultAsync();
    }

    public async Task<UserModel?> GetUserByEmail(string email) =>
        await _userCollection.Find(x => x.email == email).FirstOrDefaultAsync();

    public async Task CreateUser(UserModel newUser) =>
        await _userCollection.InsertOneAsync(newUser);

    public async Task UpdateUser(string id, UserModel updatedUser) =>
        await _userCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task RemoveUser(string id) =>
        await _userCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<UserModel?> Authenticate(string email, string password)
    {
        var user = await _userCollection.Find(x => x.email == email).FirstOrDefaultAsync();
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password))
        {
            return user;
        }
        return null;
    }

    public async Task<bool> IsEmailRegistered(string email)
    {
        var user = await _userCollection.Find(x => x.email == email).FirstOrDefaultAsync();
        return user != null;
    }
}
