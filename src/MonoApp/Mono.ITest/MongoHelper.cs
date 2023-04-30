using MongoDB.Driver;

namespace Mono.ITest;

public class MongoHelper
{
    public IMongoDatabase Database { get; }

    public static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("MongoDb__ConnectionString") ?? "mongodb://localhost:27017";

    public static readonly string DatabaseName =
        Environment.GetEnvironmentVariable("MongoDb__DatabaseName") ?? "MonoAppTest";

    public MongoHelper()
    {
        var client = new MongoClient(ConnectionString);
        Database = client.GetDatabase(DatabaseName);
    }
}