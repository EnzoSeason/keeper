using Domain.SeedWork;

namespace Configuring.Domain.SourceAggregation;

public interface ISourceRepository : IRepository<Source>
{
    Task InsertOne(Source source);
    
    Task<bool> IsFound(Guid configId);
}