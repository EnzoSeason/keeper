using MongoDB.Driver;

namespace Reporting.ITest;

public record MongoDbTestInstance
{
    public IMongoDatabase Database { get; }

    public static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("MongoDb__ConnectionString") ?? "mongodb://localhost:27017";

    public static readonly string DatabaseName =
        Environment.GetEnvironmentVariable("MongoDb__DatabaseName") ?? "KeeperReportingTest";

    public static readonly string TransactionCollectionName =
        Environment.GetEnvironmentVariable("MongoDb__TransactionCollectionName") ?? "TransactionTest";

    public MongoDbTestInstance()
    {
        var client = new MongoClient(ConnectionString);
        Database = client.GetDatabase(DatabaseName);
    }
}