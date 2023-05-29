using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Configuring.Infrastructure.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string SourceCollectionName { get; set; } = null!;
}

public static class MongoDbHelper
{
    public static IMongoDatabase GetDatabase(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(
            mongoDbSettings.Value.ConnectionString);
        
        return mongoClient.GetDatabase(
            mongoDbSettings.Value.DatabaseName);
    }
}