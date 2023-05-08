using Reporting.Domain.AggregatesModel.TransactionAggregate;

namespace Reporting.Infrastructure.Repositories;

public interface ITransactionRepository
{
    Task InsertTransaction(Transaction transaction);
}

public class TransactionRepository: ITransactionRepository
{
    public Task InsertTransaction(Transaction transaction)
    {
        throw new NotImplementedException();
    }
}