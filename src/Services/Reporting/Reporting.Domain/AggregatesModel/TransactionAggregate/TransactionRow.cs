using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Reporting.Domain.AggregatesModel.TransactionAggregate;

public record TransactionRow: IValidatableObject
{
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Date { get; set; }
    
    public string Label { get; set; }
    
    public double Amount { get; set; }

    public string Currency { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (string.IsNullOrEmpty(Label))
        {
            results.Add(new ValidationResult("Label is required.", new[] { nameof(Label) }));
        }

        if (Amount == 0)
        {
            results.Add(new ValidationResult("Amount should not be 0.", new[] { nameof(Amount) }));
        }

        if (string.IsNullOrEmpty(Currency))
        {
            results.Add(new ValidationResult("Currency is required.", new[] { nameof(Currency) }));
        }
        
        return results;
    }
}