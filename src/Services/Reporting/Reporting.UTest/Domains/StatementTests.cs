using NUnit.Framework;
using Reporting.Domain.AggregatesModel.StatementAggregate;
using Reporting.Domain.AggregatesModel.TransactionAggregate;
using Reporting.Domain.AggregatesModel.ValueObjects;

namespace Reporting.UTest.Domains;

public class StatementTests
{
    [Test]
    public void BuildFromTransactions()
    {
        var rows = new[]
        {
            new TransactionRow
            {
                Label = "a", Amount = 1m, Currency = "EUR"
            },
            new TransactionRow
            {
                Label = "b", Amount = 2m, Currency = "EUR"
            },
            new TransactionRow
            {
                Label = "c", Amount = 3m, Currency = "EUR"
            }
        };

        var transactions = new[]
        {
            new Transaction
            {
                Rows = rows.Take(2)
            },
            new Transaction
            {
                Rows = rows.TakeLast(1)
            }
        };

        var statement = Statement.Build(Guid.NewGuid(), 2023, 3, transactions);
        
        Assert.That(statement.Rows.SequenceEqual(rows));
    }
}