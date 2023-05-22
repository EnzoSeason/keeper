namespace Reporting.Domain.TransactionAggregate;

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