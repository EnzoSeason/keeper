using Domain.SeedWork;

namespace Configuring.Domain.SourceAggregation;

public interface ISourceRepository : IRepository<Source>
{
    Task<bool> IsFound(Guid configId);
    
    Task InsertOne(Source source);

    Task<Source?> Get(Guid configId);

    Task<bool> ReplaceOne(Guid configId, Source source);
}