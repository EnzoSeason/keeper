using Configuring.Domain.SourceAggregation;

namespace Configuring.Infrastructure.Repositories;

public class SourceRepository : ISourceRepository
{
    public Task InsertOne(Source source)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsFound(Guid configId)
    {
        throw new NotImplementedException();
    }
}