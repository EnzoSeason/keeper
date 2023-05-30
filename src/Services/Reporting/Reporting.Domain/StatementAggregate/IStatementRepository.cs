using Domain.SeedWork;

namespace Reporting.Domain.StatementAggregate;

public interface IStatementRepository: IRepository<Statement>
{
    Task<bool> IsFound(Guid configId, int year, int month);

    Task<Statement> AggregateTransactions(Guid configId, int year, int month);
}