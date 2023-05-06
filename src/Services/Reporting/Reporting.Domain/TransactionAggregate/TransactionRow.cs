using System.ComponentModel.DataAnnotations;

namespace Reporting.Domain.TransactionAggregate;

public class TransactionRow: IValidatableObject
{
    public DateTime Date { get; set; }
    
    public string Label { get; set; }
    
    public double Amount { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (string.IsNullOrEmpty(Label))
        {
            results.Add(new ValidationResult("Label is required.", new[] { nameof(Label) }));
        }

        // Return the validation results
        return results;
    }
}