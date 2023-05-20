using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Reporting.Domain.AggregatesModel.ValueObjects;
using Reporting.Domain.SeedWork;

namespace Reporting.Domain.AggregatesModel.StatementAggregate;

public record Statement: IAggregateRoot, IValidatableObject
{
    public ObjectId Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid ConfigId { get; init; }
    
    public int Year { get; init; }
    
    public int Month { get; init; }
    
    public IEnumerable<TransactionRow> Rows { get; init; } = null!;
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => Rows.Validate(Year, Month);

    public virtual bool Equals(Statement? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ConfigId.Equals(other.ConfigId) && 
               Year == other.Year && 
               Month == other.Month && 
               Rows.SequenceEqual(other.Rows);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ConfigId, Year, Month, Rows);
    }
}