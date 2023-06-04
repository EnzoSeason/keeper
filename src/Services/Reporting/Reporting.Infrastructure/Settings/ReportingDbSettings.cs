using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Reporting.Infrastructure.Settings;

public class ReportingDbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string TransactionCollectionName { get; set; } = null!;

    public string StatementCollectionName { get; set; } = null!;

    public string ReportCollectionName { get; set; } = null!;
}

public static class ReportingDbHelper
{
    public static IMongoDatabase GetDatabase(IOptions<ReportingDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(
            mongoDbSettings.Value.ConnectionString);
        
        return mongoClient.GetDatabase(
            mongoDbSettings.Value.DatabaseName);
    }
}