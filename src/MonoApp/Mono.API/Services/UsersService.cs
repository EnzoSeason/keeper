using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Mono.API.Models;

namespace Mono.API.Services;

public interface IUsersService
{
    Task<IEnumerable<User>> GetUsers();
    Task<User> GetUser(Guid uid);
    Task CreateUser(User user);
}

public class UsersService: IUsersService
{
    private readonly IMongoCollection<User> _usersCollection;
    
    public UsersService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(
            mongoDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            mongoDbSettings.Value.DatabaseName);
        
        _usersCollection = mongoDatabase.GetCollection<User>("Users");
    }

    public async Task<IEnumerable<User>> GetUsers() => await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<User> GetUser(Guid uid) =>
        await _usersCollection.Find(user => user.Uid == uid).FirstOrDefaultAsync();

    public async Task CreateUser(User user) => await _usersCollection.InsertOneAsync(user);
}