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

    public string SourceCollectionName { get; set; } = null!;
}

public static class ReportingDbHelper
{
    public static IMongoDatabase GetDatabase(IOptions<ReportingDbSettings> reportingDbSettings)
    {
        var mongoClient = new MongoClient(
            reportingDbSettings.Value.ConnectionString);
        
        return mongoClient.GetDatabase(
            reportingDbSettings.Value.DatabaseName);
    }
}