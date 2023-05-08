using Reporting.Domain.SeedWork;

namespace Reporting.Domain.AggregatesModel.TransactionAggregate;

public interface ITransactionRepository: IRepository<Transaction>
{
    Task InsertOne(Transaction transaction);
}