using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Configuring.Infrastructure.Settings;

public class ConfiguringDbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string SourceCollectionName { get; set; } = null!;
}

public static class ConfiguringDbHelper
{
    public static IMongoDatabase GetDatabase(IOptions<ConfiguringDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(
            mongoDbSettings.Value.ConnectionString);
        
        return mongoClient.GetDatabase(
            mongoDbSettings.Value.DatabaseName);
    }
}