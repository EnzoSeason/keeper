using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Reporting.Domain.TransactionModels;

/// <summary>
/// The document of Transaction saved in the MongoDB
/// </summary>
public record TransactionDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid ConfigId { get; init; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime FileDate { get; init; }

    public List<TransactionRowDocument> Rows { get; init; }

    public virtual bool Equals(TransactionDocument? other)
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

/// <summary>
/// The document of Transaction Row saved in the MongoDB
/// </summary>
public record TransactionRowDocument
{
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Date { get; init; }
    
    public string Label { get; init; }
    
    public double Amount { get; init; }

    public string Currency { get; init; }
}