using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Mono.ITest;

public class MongoHelper
{
    public IMongoDatabase Database { get; }

    public static readonly string ConnectionString = "mongodb://admin:pass@localhost:27018";

    public static readonly string DatabaseName = "MonoAppTest";

    public MongoHelper()
    {
        var client = new MongoClient(ConnectionString);
        Database = client.GetDatabase(DatabaseName);
    }
}