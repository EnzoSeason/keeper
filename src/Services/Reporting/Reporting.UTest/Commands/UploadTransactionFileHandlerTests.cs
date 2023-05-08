using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using Reporting.API.Commands.UploadTransactionFile;
using Reporting.Domain.AggregatesModel.TransactionAggregate;

namespace Reporting.UTest.Commands;

public class UploadTransactionFileHandlerTests
{
    private IFormFile _file;
    private ITransactionRepository _repository;
    private UploadTransactionFileHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _file = Substitute.For<IFormFile>();
        _repository = Substitute.For<ITransactionRepository>();
        _handler = new UploadTransactionFileHandler(_repository);
    }

    [Test]
    public async Task InvalidTransactionRow_ReturnTrue()
    {
        // Only the first two rows are valid.
        const string rows = @"
Date;Label;Amount;Currency;
29/03/2023;FRANPRIX;-6,15;EUR;
29/03/2023;PYMT;16,08;EUR;
29/03/2023;;-16,08;EUR;
29/03/2023;LIDL;0;EUR;
29/03/2023;LIDL;-16,08;;
";
        var stream = GetStream(rows);
        _file.OpenReadStream().Returns(stream);

        var configId = Guid.NewGuid();
        var fileDate = DateTime.Parse("2023-03-29");
        var command = new UploadTransactionFileCommand
        {
            ConfigId = configId,
            FileDate = fileDate,
            File = _file
        };

        var expectedTransaction = new Transaction
        {
            ConfigId = configId,
            FileDate = fileDate,
            Rows = new List<TransactionRow>
            {
                new()
                {
                    Date = DateTime.Parse("2023-03-29"),
                    Label = "FRANPRIX",
                    Amount = -6.15,
                    Currency = "EUR"
                },
                new()
                {
                    Date = DateTime.Parse("2023-03-29"),
                    Label = "PYMT",
                    Amount = 16.08,
                    Currency = "EUR"
                }
            }
        };

        var response = await _handler.Handle(command, CancellationToken.None);
        
        Assert.That(response, Is.True);
        await _repository.Received(1).InsertOne(expectedTransaction);
    }

    [TestCaseSource(nameof(GetInvalidTransactionTestCases))]
    public async Task InvalidTransaction_ReturnFalse(Stream stream, DateTime fileDate)
    {
        _file.OpenReadStream().Returns(stream);
        
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            FileDate = fileDate,
            File = _file
        };
        
        var response = await _handler.Handle(command, CancellationToken.None);
        
        Assert.That(response, Is.False);
        await _repository.Received(0).InsertOne(Arg.Any<Transaction>());
    }

    [Test]
    public async Task RepositoryException_ReturnFalse()
    {
        const string normalRows = @"
Date;Label;Amount;Currency;
29/03/2023;FRANPRIX;-6,15;EUR;
";
        var stream = GetStream(normalRows);
        _file.OpenReadStream().Returns(stream);

        _repository.InsertOne(Arg.Any<Transaction>()).Returns(_ => throw new Exception());
        
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            FileDate = DateTime.Parse("2023-03-29"),
            File = _file
        };
        
        var response = await _handler.Handle(command, CancellationToken.None);
        
        Assert.That(response, Is.False);
    }

    [Test]
    public async Task CancellationRequested_ReturnFalse()
    {
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            FileDate = DateTime.Parse("2023-03-29"),
            File = _file
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();
        
        var response = await _handler.Handle(command, cts.Token);
        Assert.That(response, Is.False);
    }

    private static IEnumerable<TestCaseData> GetInvalidTransactionTestCases()
    {
        var fileDate = DateTime.Parse("2023-03-29");
        var emptyRowStream = new MemoryStream();
        yield return new TestCaseData(emptyRowStream, fileDate).SetName("Transaction rows are empty.");
        
        const string differentMonthsRows = @"
Date;Label;Amount;Currency;
29/03/2023;FRANPRIX;-6,15;EUR;
29/04/2023;PYMT;16,08;EUR;
";
        var differentMonthsStream = GetStream(differentMonthsRows);
        yield return new TestCaseData(differentMonthsStream, fileDate).SetName("Transaction rows are not in the same month.");
        
        const string normalRows = @"
Date;Label;Amount;Currency;
29/03/2023;FRANPRIX;-6,15;EUR;
";
        var normalStream = GetStream(normalRows);
        yield return new TestCaseData(normalStream, DateTime.Parse("2023-04-29"))
            .SetName("The date in transaction rows is different from the file date.");
    }

    private static MemoryStream GetStream(string data)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        writer.Write(data);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }
}