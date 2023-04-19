using System.Net;
using Microsoft.Extensions.Configuration;
using Mono.API.Models;
using Xunit;

namespace Mono.ITest.Controllers;

public class UserTests: IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private const string Uid1 = "f994d51c-d47e-482c-9b81-6577da89db7e";
    private const string Uid2 = "4d4bb79d-0916-41c0-83c1-36dddbd84a9d";
    private const string UnknownUid = "db056975-0d02-4cc5-99be-8d6238cebddd";
    
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

        var user1 = new User(Guid.Parse(Uid1), "dummy");
        var user2 = new User(Guid.Parse(Uid2), "random");
        userCollection.InsertMany(new[] { user1, user2 });
    }


    public void Dispose()
    {
        _mongoHelper.Database.DropCollection("Users");
    }

    [Theory]
    [InlineData("/api/v1/user")]
    [InlineData("/api/v1/user/" + Uid1)]
    public async Task Get_Success(string url)
    {
        var response = await _client.GetAsync(url);
        
        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData("/api/v1/user/" + UnknownUid)]
    public async Task Get_Not_Found(string url)
    {
        var response = await _client.GetAsync(url);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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