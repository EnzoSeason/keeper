using NSubstitute;
using NUnit.Framework;
using Reporting.API.Commands.AggregateTransactions;
using Reporting.Domain.StatementAggregate;

namespace Reporting.UTest.Commands;

public class AggregateTransactionsHandlerTests
{
    private IStatementRepository _repository;
    private AggregateTransactionsHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IStatementRepository>();
        _handler = new AggregateTransactionsHandler(_repository);
    }
    
    [Test]
    public async Task CancellationRequested_ReturnFalse()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        
        var response = await _handler.Handle(new AggregateTransactionsCommand(), cts.Token);
        Assert.That(response, Is.False);
    }
    
    [Test]
    public async Task StatementIsFound_ReturnFalse()
    {
        var configId = Guid.NewGuid();
        var year = 2023;
        var month = 3;

        var command = new AggregateTransactionsCommand
        {
            ConfigId = configId,
            Year = year,
            Month = month
        };

        _repository.IsFound(configId, year, month).Returns(Task.FromResult(true));

        var response = await _handler.Handle(command, CancellationToken.None);
        
        Assert.That(response, Is.False);
    }

    [Test]
    public async Task CreateStatementSuccess_ReturnTrue()
    {
        var configId = Guid.NewGuid();
        var year = 2023;
        var month = 3;

        var command = new AggregateTransactionsCommand
        {
            ConfigId = configId,
            Year = year,
            Month = month
        };
        
        var response = await _handler.Handle(command, CancellationToken.None);
        
        Assert.That(response, Is.True);
    }
}