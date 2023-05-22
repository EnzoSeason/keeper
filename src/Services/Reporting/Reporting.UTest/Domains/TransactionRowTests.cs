using Domain.SeedWork;
using NUnit.Framework;
using Reporting.Domain.ValueObjects;

namespace Reporting.UTest.Domains;

public class TransactionRowTests
{
    [Test]
    public void Label_ShouldNotBeNullOrEmpty()
    {
        var row = new TransactionRow
        {
            Date = DateTime.UtcNow,
            Amount = 1m,
            Currency = "EUR"
        };

        Assert.That(DomainModelValidator<TransactionRow>.TryValidate(row, out var results), Is.False);
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results.First().ErrorMessage, Is.EqualTo("Label is required."));
    }
    
    [Test]
    public void Currency_ShouldNotBeNullOrEmpty()
    {
        var row = new TransactionRow
        {
            Label = "a",
            Date = DateTime.UtcNow,
            Amount = 1m,
        };

        Assert.That(DomainModelValidator<TransactionRow>.TryValidate(row, out var results), Is.False);
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results.First().ErrorMessage, Is.EqualTo("Currency is required."));
    }

    [Test]
    public void Amount_ShouldNotBeZero()
    {
        var row = new TransactionRow
        {
            Label = "a",
            Date = DateTime.UtcNow,
            Amount = 0m,
            Currency = "EUR"
        };
        
        Assert.That(DomainModelValidator<TransactionRow>.TryValidate(row, out var results), Is.False);
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results.First().ErrorMessage, Is.EqualTo("Amount should not be 0."));
    }
}