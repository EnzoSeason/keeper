using Microsoft.Extensions.Configuration;
using Mono.API.Models;
using Xunit;

namespace Mono.ITest;

public class UserTests: IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private const string UserUid = "f994d51c-d47e-482c-9b81-6577da89db7e";
    
    private readonly HttpClient _client;
    private readonly MongoHelper _mongoHelper;

    public UserTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _mongoHelper = new MongoHelper();
        
        AddUserSeed();
    }

    private void AddUserSeed()
    {
        var userCollection = _mongoHelper.Database.GetCollection<User>("Users");

        var knownUser = new User(Guid.Parse(UserUid), "dummy");
        var randomUser = new User(Guid.NewGuid(), "random");
        userCollection.InsertMany(new[] { knownUser, randomUser });
    }


    public void Dispose()
    {
        _mongoHelper.Database.DropCollection("Users");
    }

    [Theory]
    [InlineData("/api/v1/user")]
    [InlineData("/api/v1/user/f994d51c-d47e-482c-9b81-6577da89db7e")]
    public async Task Get_Success(string url)
    {
        var response = await _client.GetAsync(url);
        
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("/api/v1/user?username=toto")]
    public async Task Post_Success(string url)
    {
        var response = await _client.PostAsync(url, null);
        
        response.EnsureSuccessStatusCode();
    }

    private static IConfiguration GetConfiguration()
    {
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.json");
        
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }
}