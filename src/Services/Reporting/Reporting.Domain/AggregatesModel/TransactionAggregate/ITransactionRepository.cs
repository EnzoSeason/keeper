namespace Reporting.Domain.AggregatesModel.TransactionAggregate;

public interface ITransactionRepository
{
    Task InsertOne(Transaction transaction);
}