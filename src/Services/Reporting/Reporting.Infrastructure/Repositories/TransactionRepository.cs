using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
    
    public async Task<bool> IsFound(Guid configId, int year, int month, Origin origin)
    {
        var filter = Builders<Transaction>.Filter.And(
            Builders<Transaction>.Filter.Eq(t => t.ConfigId, configId), 
            Builders<Transaction>.Filter.Eq(t => t.Year, year),
            Builders<Transaction>.Filter.Eq(t => t.Month, month),
            Builders<Transaction>.Filter.Eq(t => t.Origin, origin));
        
        var results = await _transactionCollection.FindAsync(filter);

        return await results.AnyAsync();
    }
}