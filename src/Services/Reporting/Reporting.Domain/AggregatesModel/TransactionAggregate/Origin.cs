using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Reporting.Domain.AggregatesModel.TransactionAggregate;

public record Origin
{
    public OriginType Type { get; init; }
    public string Description { get; init; } = null!;
}

public enum OriginType
{
    Unknown,
    File,
    Api,
}