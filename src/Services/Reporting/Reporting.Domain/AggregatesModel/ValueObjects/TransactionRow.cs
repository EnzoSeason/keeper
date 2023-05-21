using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Reporting.Domain.AggregatesModel.ValueObjects;

public record TransactionRow: IValidatableObject
{
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Date { get; init; }
    
    public string Label { get; init; } = null!;

    public decimal Amount { get; init; }

    public string Currency { get; init; } = null!;

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