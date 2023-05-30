namespace Reporting.Domain.ValueObjects;

public record AnalysisRow
{
    public string Label { get; init; } = null!;
    
    public AnalysisType AnalysisType { get; init; }

    public decimal Amount { get; init; }

    public string Currency { get; init; } = null!;
}

// TODO: Replace the hardcoded Type by the configuring service
public enum AnalysisType
{
    Unknown,
    Sum
}