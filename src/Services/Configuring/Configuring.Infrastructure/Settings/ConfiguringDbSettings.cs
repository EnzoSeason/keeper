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
    public static IMongoDatabase GetDatabase(IOptions<ConfiguringDbSettings> configuringDbSettings)
    {
        var mongoClient = new MongoClient(
            configuringDbSettings.Value.ConnectionString);
        
        return mongoClient.GetDatabase(
            configuringDbSettings.Value.DatabaseName);
    }
}