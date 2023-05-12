using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Reporting.Domain.SeedWork;

namespace Reporting.Domain.AggregatesModel.TransactionAggregate;

public record Transaction: IAggregateRoot, IValidatableObject
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid ConfigId { get; init; }
    
    public int Year { get; init; }
    
    public int Month { get; init; }

    /// <summary>
    /// The creation datetime in millisecond 
    /// </summary>
    public long Version { get; init; }
    
    public Origin Origin { get; init; } = null!;

    public IEnumerable<TransactionRow> Rows { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (!Rows.Any())
        {
            results.Add(new ValidationResult("Rows must contain data.", new[] { nameof(Rows) }));
            return results;
        }

        var rowsYears = Rows.Select(row => row.Date.Year).Distinct().ToList();
        var rowsMonths = Rows.Select(row => row.Date.Month).Distinct().ToList();

        if (rowsYears.Count != 1 || rowsMonths.Count != 1)
        {
            results.Add(
                new ValidationResult("Rows must be the transactions of the same month in the same year.",
                    new[] { nameof(Rows) }));
            return results;
        }
        
        if (rowsYears.First() != Year)
        {
            results.Add(
                new ValidationResult("The transaction year in rows doesn't match to the one in metadata.",
                    new[] { nameof(Rows) }));
        }

        if (rowsMonths.First() != Month)
        {
            results.Add(
                new ValidationResult("The transaction month in rows doesn't match to the one in metadata.",
                    new[] { nameof(Rows) }));
        }

        return results;
    }
}