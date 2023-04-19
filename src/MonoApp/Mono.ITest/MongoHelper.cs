using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Mono.ITest;

public class MongoHelper
{
    public IMongoDatabase Database { get; }

    public const string ConnectionString = "mongodb://admin:pass@localhost:27018";

    public const string DatabaseName = "MonoAppTest";

    public MongoHelper()
    {
        var client = new MongoClient(ConnectionString);
        Database = client.GetDatabase(DatabaseName);
    }
}