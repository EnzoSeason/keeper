using Domain.SeedWork;

namespace Reporting.Domain.AggregatesModel.TransactionAggregate;

public interface ITransactionRepository: IRepository<Transaction>
{
    Task InsertOne(Transaction transaction);

    Task<bool> IsFound(Guid configId, int year, int month, Origin origin);
}