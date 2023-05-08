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
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
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

    public virtual bool Equals(Transaction? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && ConfigId.Equals(other.ConfigId) && FileDate.Equals(other.FileDate) &&
               Rows.SequenceEqual(other.Rows);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ConfigId, FileDate, Rows);
    }
}