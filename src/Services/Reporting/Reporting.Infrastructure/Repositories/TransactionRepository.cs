using Reporting.Domain.TransactionModels;

namespace Reporting.Infrastructure.Repositories;

public interface ITransactionRepository
{
    Task InsertTransaction(TransactionDocument transaction);
}

public class TransactionRepository: ITransactionRepository
{
    public Task InsertTransaction(TransactionDocument transaction)
    {
        throw new NotImplementedException();
    }
}