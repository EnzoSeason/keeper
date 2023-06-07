using Configuring.Domain.SourceAggregation;
using Domain.SeedWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Reporting.Domain.StatementAggregate;
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

    public static Report Analyze(Source source, Statement statement)
    {
        // TODO: replace hardcoded Analysis row by configuration 
        var mapping = source.Categories.ToDictionary(
            c => c.Keywords, 
            c => new AnalysisRow { Label = c.Name, AnalysisType = AnalysisType.Sum, Currency = "EUR" });

        var others = new AnalysisRow { Label = "Others", AnalysisType = AnalysisType.Sum, Currency = "EUR", Amount = 0m};

        foreach (var transactionRow in statement.Rows)
        {
            var isClassified = false;
            foreach (var (keywords, analysisRow) in mapping)
            {
                if (isClassified) { break; }
                
                // TODO: find more indicators for classifying transaction rows
                var isInCategory = transactionRow.Label
                    .Split(" ")
                    .Where(word => !string.IsNullOrEmpty(word))
                    .Select(word => word.ToLower())
                    .Any(word => keywords.Contains(word));
                
                if (isInCategory)
                {
                    analysisRow.Amount += transactionRow.Amount;
                    isClassified = true;
                }
            }

            if (!isClassified)
            {
                others.Amount += transactionRow.Amount;
            }
        }

        return new Report
        {
            ConfigId = statement.ConfigId,
            Year = statement.Year,
            Month = statement.Month,
            Rows = mapping.Values.Append(others)
        };
    }
}