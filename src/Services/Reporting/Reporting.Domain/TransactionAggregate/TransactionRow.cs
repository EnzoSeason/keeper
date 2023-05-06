namespace Reporting.Domain.TransactionAggregate;

public class TransactionRow
{
    public DateTime Date { get; set; }
    
    public string Label { get; set; }
    
    public double amount { get; set; }
}