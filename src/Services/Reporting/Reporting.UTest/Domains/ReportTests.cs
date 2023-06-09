using Configuring.Domain.SourceAggregation;
using NUnit.Framework;
using Reporting.Domain.ReportAggregate;
using Reporting.Domain.StatementAggregate;
using Reporting.Domain.ValueObjects;

namespace Reporting.UTest.Domains;

public class ReportTests
{
    [Test]
    public void Analyze()
    {
        var source = GetSource();
        var statement = GetStatement();

        var report = Report.Analyze(source, statement);
        
        Assert.Multiple(() =>
        {
            Assert.That(report.ConfigId, Is.EqualTo(statement.ConfigId));
            Assert.That(report.Year, Is.EqualTo(statement.Year));
            Assert.That(report.Month, Is.EqualTo(statement.Month));
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(report.Rows.Count(), Is.EqualTo(3));
            
            AssertAnalysisRow(report.Rows, "Daily Spend", -38.18m);
            AssertAnalysisRow(report.Rows, "Subscription", -94m);
            AssertAnalysisRow(report.Rows, "Others", 100m);
        });
    }

    private static void AssertAnalysisRow(IEnumerable<AnalysisRow> rows, string label, decimal expectedAmount)
    {
        var row = rows.FirstOrDefault(r => r.Label == label);
        
        Assert.That(row, Is.Not.Null);
        Assert.That(row!.Amount, Is.EqualTo(expectedAmount));
    }

    private static Source GetSource() => new()
    {
        Categories = new[]
        {
            new Category
            {
                Name = "Daily Spend",
                Keywords = new[] { "supermarket", "restaurant" }
            },
            new Category
            {
                Name = "Subscription",
                Keywords = new[] { "transport", "phone" }
            }
        }
    };

    private static Statement GetStatement() => new()
    {
        ConfigId = Guid.NewGuid(),
        Year = 2023,
        Month = 3,
        Rows = new[]
        {
            new TransactionRow
            {
                Label = "asian restaurant",
                Amount = -25.83m
            },
            new TransactionRow
            {
                Label = "monoprix supermarket",
                Amount = -12.35m
            },
            new TransactionRow
            {
                Label = "my phone fee",
                Amount = -9.99m
            },
            new TransactionRow
            {
                Label = "transport paris",
                Amount = -84.01m
            },
            new TransactionRow
            {
                Label = "something crazy",
                Amount = 1000m
            },
            new TransactionRow
            {
                Label = "something bad",
                Amount = -900m
            }
        }
    };
}