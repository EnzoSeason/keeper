using System.ComponentModel.DataAnnotations;
using Reporting.Domain.SeedWork;

namespace Reporting.Domain.AggregatesModel.TransactionAggregate;

public class Transaction: IAggregateRoot, IValidatableObject
{
    public Guid ConfigId { get; init; }
    
    public DateTime FileDate { get; init; }

    public IList<TransactionRow> Rows { get; set; } = new List<TransactionRow>();
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (!Rows.Any())
        {
            results.Add(new ValidationResult("Rows must contain data.", new[] { nameof(Rows) }));
            return results;
        }

        var rowsMonths = Rows.Select(row => row.Date.Month).Distinct().ToList();

        if (rowsMonths.Count != 1)
        {
            results.Add(
                new ValidationResult("Rows must be the transactions of the same month.", new[] { nameof(Rows) }));
            return results;
        }

        if (rowsMonths.First() != FileDate.Month)
        {
            results.Add(
                new ValidationResult("The transaction date in rows doesn't match to the file date.",
                    new[] { nameof(Rows) }));
        }

        return results;
    }
}