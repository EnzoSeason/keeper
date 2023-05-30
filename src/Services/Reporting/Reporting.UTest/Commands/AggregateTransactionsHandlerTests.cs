using NSubstitute;
using NUnit.Framework;
using Reporting.API.Commands.AggregateTransactions;
using Reporting.Domain.ReportAggregate;
using Reporting.Domain.StatementAggregate;

namespace Reporting.UTest.Commands;

public class AggregateTransactionsHandlerTests
{
    private IStatementRepository _statementRepository;
    private IReportRepository _reportRepository;
    private AggregateTransactionsHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _statementRepository = Substitute.For<IStatementRepository>();
        _reportRepository = Substitute.For<IReportRepository>();
        _handler = new AggregateTransactionsHandler(_statementRepository, _reportRepository);
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

        _statementRepository.IsFound(configId, year, month).Returns(Task.FromResult(true));

        var response = await _handler.Handle(command, CancellationToken.None);
        
        Assert.That(response, Is.False);
    }

    [Test]
    public async Task Success_ReturnTrue()
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