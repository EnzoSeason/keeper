using Reporting.Domain.AggregatesModel.TransactionAggregate;

namespace Reporting.Infrastructure.Repositories;

public class TransactionRepository: ITransactionRepository
{
    public Task InsertOne(Transaction transaction)
    {
        throw new NotImplementedException();
    }
}