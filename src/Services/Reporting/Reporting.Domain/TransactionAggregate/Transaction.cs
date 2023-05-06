using Reporting.Domain.SeedWork;

namespace Reporting.Domain.TransactionAggregate;

public class Transaction: IAggregateRoot
{
    public Guid UserId { get; init; }
    
    public DateTime FileDate { get; init; }
    
    public IEnumerable<TransactionRow> Rows { get; set; }
}