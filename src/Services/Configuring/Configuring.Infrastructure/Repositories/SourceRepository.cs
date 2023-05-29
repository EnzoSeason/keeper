using Configuring.Domain.SourceAggregation;
using Configuring.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Configuring.Infrastructure.Repositories;

public class SourceRepository : ISourceRepository
{
    private readonly IMongoCollection<Source> _sourceCollection;

    public SourceRepository(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoDatabase = MongoDbHelper.GetDatabase(mongoDbSettings);

        _sourceCollection = mongoDatabase.GetCollection<Source>(mongoDbSettings.Value.SourceCollectionName);
    }

    public async Task<bool> IsFound(Guid configId)
    {
        var filter = Builders<Source>.Filter.Eq(s => s.ConfigId, configId);
        
        var result = await _sourceCollection.FindAsync(filter);

        return await result.AnyAsync();
    }

    public async Task InsertOne(Source source) => await _sourceCollection.InsertOneAsync(source);

    public async Task<Source?> Get(Guid configId) =>
        await _sourceCollection.Find(s => s.ConfigId == configId).FirstOrDefaultAsync();

    public async Task<bool> ReplaceOne(Guid configId, Source source)
    {
        var filter = Builders<Source>.Filter.Eq(s => s.ConfigId, configId);
        var oldSource = await _sourceCollection.Find(filter).FirstOrDefaultAsync();

        if (oldSource is null)
        {
            return false;
        }
        
        source.Id = oldSource.Id;
        await _sourceCollection.ReplaceOneAsync(filter, source);
        return true;
    }
}