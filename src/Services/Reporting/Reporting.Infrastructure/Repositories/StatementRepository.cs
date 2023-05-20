using Reporting.Domain.AggregatesModel.StatementAggregate;

namespace Reporting.Infrastructure.Repositories;

public class StatementRepository : IStatementRepository
{
    public Task<bool> IsFound(Guid configId, int year, int month)
    {
        throw new NotImplementedException();
    }

    public Task AggregateTransactions(Guid configId, int year, int month)
    {
        throw new NotImplementedException();
    }
}