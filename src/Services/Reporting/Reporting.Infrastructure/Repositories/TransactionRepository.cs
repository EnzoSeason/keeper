using Reporting.Domain.TransactionModels;

namespace Reporting.Infrastructure.Repositories;

public interface ITransactionRepository
{
    Task InsertTransaction(TransactionEntity transaction);
}

public class TransactionRepository: ITransactionRepository
{
    public Task InsertTransaction(TransactionEntity transaction)
    {
        throw new NotImplementedException();
    }
}