using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Rain.Models;
using BCrypt.Net;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;

namespace Rain.Services
{
    public class UserService
    {
        private readonly IMongoCollection<UserModel> _userCollection;
        private readonly ILogger<UserService> _logger;

        public UserService(IOptions<DatabaseSettings> databaseSettings, ILogger<UserService> logger)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.RainWebDatabase.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.RainWebDatabase.DatabaseName);
            _userCollection = mongoDatabase.GetCollection<UserModel>(databaseSettings.Value.RainWebDatabase.CollectionName);
            _logger = logger;
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

        public async Task CreateUser(UserModel newUser)
        {
            _logger.LogInformation("Creating user with email: {Email}", newUser.email);

            // Insert or replace the user document with the same email
            var existingUser = await _userCollection.Find(x => x.email == newUser.email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                _logger.LogInformation("User already exists, updating the existing user.");
                newUser.Id = existingUser.Id;  // Retain the same Id to update the document
                await _userCollection.ReplaceOneAsync(x => x.email == newUser.email, newUser);
            }
            else
            {
                await _userCollection.InsertOneAsync(newUser);
            }

            _logger.LogInformation("User created/updated with email: {Email}", newUser.email);
        }


        public async Task UpdateUser(string id, UserModel updatedUser) =>
            await _userCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

        public async Task RemoveUser(string id) =>
            await _userCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<UserModel?> Authenticate(string email, string password)
        {
            var user = await _userCollection.Find(x => x.email == email).FirstOrDefaultAsync();
            if (user != null)
            {
                _logger.LogInformation("User found with email: {Email}. Stored hashed password: {StoredPassword}", email, user.password);

                if (BCrypt.Net.BCrypt.Verify(password, user.password))
                {
                    _logger.LogInformation("Authentication successful for email: {Email}", email);
                    return user;
                }
                else
                {
                    _logger.LogWarning("Password verification failed for email: {Email}", email);
                }
            }
            else
            {
                _logger.LogWarning("No user found with email: {Email}", email);
            }
            return null;
        }



        public async Task HashExistingPasswords()
        {
            var users = await _userCollection.Find(_ => true).ToListAsync();
            foreach (var user in users)
            {
                try
                {
                    if (!BCrypt.Net.BCrypt.Verify("knownIncorrectPassword", user.password))
                    {
                        user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
                        await _userCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
                    }
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
                    await _userCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
                }
            }
        }
    }
}
