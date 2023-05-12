using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using Reporting.API.Commands.UploadTransactionFile;
using Reporting.API.Utils;
using Reporting.Domain.AggregatesModel.TransactionAggregate;
using Reporting.UTest.Extensions;

namespace Reporting.UTest.Commands;

public class UploadTransactionFileHandlerTests
{
    private IFormFile _file;
    private ITransactionRepository _repository;
    private IClock _clock;
    private UploadTransactionFileHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _file = Substitute.For<IFormFile>();
        _repository = Substitute.For<ITransactionRepository>();
        _clock = Substitute.For<IClock>();
        _clock.Now.Returns(DateTime.Parse("2023/03/03"));
        
        _handler = new UploadTransactionFileHandler(_repository, _clock);
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
        _file.FileName.Returns("file.csv");

        var configId = Guid.NewGuid();
        var command = new UploadTransactionFileCommand
        {
            ConfigId = configId,
            Year = 2023,
            Month = 3,
            File = _file
        };

        var expectedTransaction = new Transaction
        {
            ConfigId = configId,
            Year = 2023,
            Month = 3,
            Version = new DateTimeOffset(_clock.Now).ToUnixTimeMilliseconds(),
            Origin = new Origin
            {
                Type = OriginType.File,
                Description = _file.FileName
            },
            Rows = new List<TransactionRow>
            {
                new()
                {
                    Date = DateTime.Parse("2023-03-29"),
                    Label = "FRANPRIX",
                    Amount = -6.15M,
                    Currency = "EUR"
                },
                new()
                {
                    Date = DateTime.Parse("2023-03-29"),
                    Label = "PYMT",
                    Amount = 16.08M,
                    Currency = "EUR"
                }
            }
        };

        var received = false;
        _repository.When(r => r.InsertOne(Arg.Is<Transaction>(t => t.TransactionEquals(expectedTransaction))))
            .Do(_ => received = true);

        var response = await _handler.Handle(command, CancellationToken.None);
        
        Assert.That(response, Is.True);
        Assert.That(received, Is.True);
    }

    [TestCaseSource(nameof(GetInvalidTransactionTestCases))]
    public async Task InvalidTransaction_ReturnFalse(Stream stream, int year, int month)
    {
        _file.OpenReadStream().Returns(stream);
        
        var command = new UploadTransactionFileCommand
        {
            ConfigId = Guid.NewGuid(),
            Year = year,
            Month = month,
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
            Month = 3,
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
            Month = 3,
            File = _file
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();
        
        var response = await _handler.Handle(command, cts.Token);
        Assert.That(response, Is.False);
    }

    private static IEnumerable<TestCaseData> GetInvalidTransactionTestCases()
    {
        const int transactionYear = 2023;
        const int transactionMonth = 3;
        
        var emptyRowStream = new MemoryStream();
        yield return new TestCaseData(emptyRowStream, transactionYear, transactionMonth)
            .SetName("Transaction rows are empty.");
        
        const string differentYearsRows = @"
Date;Label;Amount;Currency;
29/03/2023;FRANPRIX;-6,15;EUR;
29/03/2024;PYMT;16,08;EUR;
";
        var differentYearsStream = GetStream(differentYearsRows);
        yield return new TestCaseData(differentYearsStream, transactionYear, transactionMonth)
            .SetName("Transaction rows are not in the same year.");
        
        const string differentMonthsRows = @"
Date;Label;Amount;Currency;
29/03/2023;FRANPRIX;-6,15;EUR;
29/04/2023;PYMT;16,08;EUR;
";
        var differentMonthsStream = GetStream(differentMonthsRows);
        yield return new TestCaseData(differentMonthsStream, transactionYear, transactionMonth)
            .SetName("Transaction rows are not in the same month.");
        
        const string normalRowsV1 = @"
Date;Label;Amount;Currency;
29/03/2023;FRANPRIX;-6,15;EUR;
";
        var normalStreamV1 = GetStream(normalRowsV1);
        yield return new TestCaseData(normalStreamV1, 2024, transactionMonth)
            .SetName("The year in transaction rows is different from the one in metadata.");
        
        const string normalRowsV2 = @"
Date;Label;Amount;Currency;
29/03/2023;FRANPRIX;-6,15;EUR;
";
        var normalStream = GetStream(normalRowsV2);
        yield return new TestCaseData(normalStream, transactionYear, 4)
            .SetName("The month in transaction rows is different from the one in metadata.");
    }

    private static MemoryStream GetStream(string data)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        writer.Write(data);
        writer.Flush();
        
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}