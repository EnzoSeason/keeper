using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Reporting.Domain.AggregatesModel.StatementAggregate;
using Reporting.Domain.AggregatesModel.TransactionAggregate;
using Reporting.Domain.AggregatesModel.ValueObjects;
using Reporting.Infrastructure.Settings;

namespace Reporting.Infrastructure.Repositories;

public class StatementRepository : IStatementRepository
{
    private readonly IMongoCollection<Statement> _statementCollection;
    private readonly IMongoCollection<Transaction> _transactionCollection;

    public StatementRepository(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoDatabase = MongoDbHelper.GetDatabase(mongoDbSettings);

        
        _transactionCollection =
            mongoDatabase.GetCollection<Transaction>(mongoDbSettings.Value.TransactionCollectionName);
        
        _statementCollection = 
            mongoDatabase.GetCollection<Statement>(mongoDbSettings.Value.StatementCollectionName);
    }

    public async Task<bool> IsFound(Guid configId, int year, int month)
    {
        var filter = Builders<Statement>.Filter.And(
            Builders<Statement>.Filter.Eq(s => s.ConfigId, configId),
            Builders<Statement>.Filter.Eq(s => s.Year, year),
            Builders<Statement>.Filter.Eq(s => s.Month, month));

        var result = await _statementCollection.FindAsync(filter);

        return await result.AnyAsync();
    }

    public async Task AggregateTransactions(Guid configId, int year, int month)
    {
        var transactionsFilter = Builders<Transaction>.Filter.And(
            Builders<Transaction>.Filter.Eq(t => t.ConfigId, configId), 
            Builders<Transaction>.Filter.Eq(t => t.Year, year),
            Builders<Transaction>.Filter.Eq(t => t.Month, month));
        
        var results = await _transactionCollection.FindAsync(transactionsFilter);
        
        var transactions = await results.ToListAsync();
        var transactionRows = transactions.SelectMany(t => t.Rows);

        var statement = new Statement
        {
            ConfigId = configId,
            Year = year,
            Month = month,
            Rows = transactionRows
        };

        await _statementCollection.InsertOneAsync(statement);
    }
}