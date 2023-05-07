using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Reporting.Domain.TransactionModels;

public record TransactionEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid ConfigId { get; init; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime FileDate { get; init; }

    public List<TransactionRowEntity> Rows { get; init; }

    public virtual bool Equals(TransactionEntity? other)
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

public record TransactionRowEntity
{
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Date { get; init; }
    
    public string Label { get; init; }
    
    public double Amount { get; init; }

    public string Currency { get; init; }
}