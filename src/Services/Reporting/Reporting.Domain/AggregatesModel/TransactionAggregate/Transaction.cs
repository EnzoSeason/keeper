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
    
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid ConfigId { get; init; }
    
    public int Year { get; init; }
    
    public int Month { get; init; }

    /// <summary>
    /// The creation datetime in millisecond 
    /// </summary>
    public long Version { get; init; }
    
    public Origin Origin { get; init; }

    public IList<TransactionRow> Rows { get; set; } = new List<TransactionRow>();

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

    public virtual bool Equals(Transaction? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && 
               ConfigId.Equals(other.ConfigId) && 
               Year == other.Year && 
               Month == other.Month && 
               Version == other.Version && 
               Origin.Equals(other.Origin) && 
               Rows.SequenceEqual(other.Rows);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ConfigId, Year, Month, Version, Origin);
    }
}