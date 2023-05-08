namespace Reporting.Infrastructure.Repositories.TransactionRepository;

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