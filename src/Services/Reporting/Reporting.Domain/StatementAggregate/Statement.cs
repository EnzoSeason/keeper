using System.ComponentModel.DataAnnotations;
using Domain.SeedWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Reporting.Domain.ValueObjects;
using Reporting.Domain.TransactionAggregate;

namespace Reporting.Domain.StatementAggregate;

public record Statement: IValidatableObject, IAggregateRoot
{
    public ObjectId Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid ConfigId { get; init; }
    
    public int Year { get; init; }
    
    public int Month { get; init; }
    
    public IEnumerable<TransactionRow> Rows { get; init; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => Rows.Validate(Year, Month);

    public static Statement Build(Guid configId, int year, int month, IEnumerable<Transaction> transactions)
    {
        return new Statement
        {
            ConfigId = configId,
            Year = year,
            Month = month,
            Rows = transactions.SelectMany(NormalizeRows)
        };
    }

    private static IEnumerable<TransactionRow> NormalizeRows(Transaction transaction) =>
        from row in transaction.Rows
        let labels = row.Label
            .Split(" ").Where(word => !string.IsNullOrEmpty(word))
            .Select(word => word
                .Trim('"')
                .ToLower())  
        select row with { Label = string.Join(" ", labels) };
    
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