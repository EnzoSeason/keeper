using NUnit.Framework;
using Reporting.Domain.StatementAggregate;
using Reporting.Domain.TransactionAggregate;
using Reporting.Domain.ValueObjects;

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

    [Test]
    public void NormalizeTransactionRows()
    {
        var rows = new[]
        {
            new TransactionRow
            {
                Label = "a  b", Amount = 1m, Currency = "EUR" // extra space between words
            },
            new TransactionRow
            {
                Label = "AAA", Amount = 2m, Currency = "EUR" // words in upper case
            },
            new TransactionRow
            {
                Label = "\"c\"", Amount = 3m, Currency = "EUR" // having double quotes
            }
        };
        
        var expectedRows = new[]
        {
            new TransactionRow
            {
                Label = "a b", Amount = 1m, Currency = "EUR"
            },
            new TransactionRow
            {
                Label = "aaa", Amount = 2m, Currency = "EUR"
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
                Rows = rows
            }
        };
        
        var statement = Statement.Build(Guid.NewGuid(), 2023, 3, transactions);
        
        Assert.That(statement.Rows.SequenceEqual(expectedRows));
    }
}