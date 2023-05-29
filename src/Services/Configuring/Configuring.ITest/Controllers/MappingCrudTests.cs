using System.Text;
using Configuring.Domain.SourceAggregation;
using ITest.SeedWork;
using MongoDB.Driver;
using Newtonsoft.Json;
using Xunit;

namespace Configuring.ITest.Controllers;

public class MappingCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private const string ConfigIdStr = "3de74b1c-36db-4b19-9694-e6a213252982";
    
    private readonly HttpClient _client;
    private readonly MongoDbTestInstance _dbTestInstance;
    private readonly IMongoCollection<Source> _sourceCollection;
    
    public MappingCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _dbTestInstance = new MongoDbTestInstance();
        _sourceCollection = _dbTestInstance.GetCollection<Source>();
    }
    
    public void Dispose()
    { 
        _dbTestInstance.Client.DropDatabase(MongoDbTestInstance.DatabaseName);
    }

    [Fact]
    public async Task Create_Success()
    {
        var url = "api/v1/mapping";
        var configIdStr = "3de74b1c-36db-4b19-9694-e6a213252982";
        var request = new StringContent(GetRequestBody(configIdStr), Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync(url, request);
        
        response.EnsureSuccessStatusCode();
        
        var result = await _sourceCollection
            .Find(s => s.ConfigId == Guid.Parse(configIdStr)).FirstOrDefaultAsync();;
        Assert.True(result is not null);
    }
    
    [Fact]
    public async Task Create_AlreadyExist_Fail()
    {
        var configIdStr = "3de74b1c-36db-4b19-9694-e6a213252982";
        var source = new Source { ConfigId = Guid.Parse(configIdStr) };
        await _sourceCollection.InsertOneAsync(source);
        
        var url = "api/v1/mapping";
        var request = new StringContent(GetRequestBody(configIdStr), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, request);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetOne_Success()
    {
        var configIdStr = "3de74b1c-36db-4b19-9694-e6a213252982";
        var source = new Source { ConfigId = Guid.Parse(configIdStr) };
        await _sourceCollection.InsertOneAsync(source);
        
        var url = $"api/v1/mapping/{configIdStr}";

        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task GetOne_NotFound_Fail()
    {
        var configIdStr = "3de74b1c-36db-4b19-9694-e6a213252982";
        var url = $"api/v1/mapping/{configIdStr}";

        var response = await _client.GetAsync(url);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateOne_Success()
    {
        var configIdStr = "3de74b1c-36db-4b19-9694-e6a213252982";
        var source = new Source
        {
            ConfigId = Guid.Parse(configIdStr),
            Name = "toto"
        };
        await _sourceCollection.InsertOneAsync(source);
        
        var url = $"api/v1/mapping/{configIdStr}";
        var expectedName = "foo";
        var request = new StringContent(GetRequestBody(configIdStr, expectedName), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync(url, request);
        
        response.EnsureSuccessStatusCode();
        var result = await _sourceCollection
            .Find(s => s.ConfigId == Guid.Parse(configIdStr)).FirstOrDefaultAsync();
        Assert.True(result is not null);
        Assert.True(result.Name == expectedName);
    }
    
    [Fact]
    public async Task UpdateOne_NotFound_Fail()
    {
        var configIdStr = "3de74b1c-36db-4b19-9694-e6a213252982";
        var url = $"api/v1/mapping/{configIdStr}";
        var request = new StringContent(GetRequestBody(configIdStr), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync(url, request);
        
        Assert.False(response.IsSuccessStatusCode);
    }
    
    private static string GetRequestBody(string configId = "3de74b1c-36db-4b19-9694-e6a213252982", string name = "bank")
    {
        var body = new
        {
            ConfigId = configId,
            Name = name,
            Categories = new []
            {
                new
                {
                    Name = "daily spend",
                    keywords = new []
                    {
                        "supermarket",
                        "bread"
                    }
                },
                new
                {
                    Name = "income",
                    keywords = new []
                    {
                        "salary",
                        "rent"
                    }
                }
            }
        };
        
        return JsonConvert.SerializeObject(body);
    }
}