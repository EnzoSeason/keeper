using Domain.SeedWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Reporting.Domain.ValueObjects;

namespace Reporting.Domain.ReportAggregate;

public record Report : IAggregateRoot
{
    public ObjectId Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid ConfigId { get; init; }
    
    public int Year { get; init; }
    
    public int Month { get; init; }
    
    public IEnumerable<AnalysisRow> Rows { get; init; } = null!;
}