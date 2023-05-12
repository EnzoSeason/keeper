using MongoDB.Driver;

namespace Reporting.ITest;

public record MongoDbTestInstance
{
    public MongoClient Client { get; } = new (ConnectionString);

    public static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("MongoDb__ConnectionString") ?? "mongodb://localhost:27017";

    public static readonly string DatabaseName =
        Environment.GetEnvironmentVariable("MongoDb__DatabaseName") ?? "KeeperReportingTest";
}