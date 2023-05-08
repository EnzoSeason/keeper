using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Reporting.Domain.AggregatesModel.TransactionAggregate;
using Reporting.Infrastructure.Settings;

namespace Reporting.Infrastructure.Repositories;

public class TransactionRepository: ITransactionRepository
{
    private readonly IMongoCollection<Transaction> _transactionCollection;

    public TransactionRepository(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(
            mongoDbSettings.Value.ConnectionString);
        
        var mongoDatabase = mongoClient.GetDatabase(
            mongoDbSettings.Value.DatabaseName);

        _transactionCollection =
            mongoDatabase.GetCollection<Transaction>(mongoDbSettings.Value.TransactionCollectionName);
    }

    public async Task InsertOne(Transaction transaction) => await _transactionCollection.InsertOneAsync(transaction);
}