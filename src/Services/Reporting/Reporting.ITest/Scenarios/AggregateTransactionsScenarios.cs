using System.Net;
using System.Text;
using Configuring.Domain.SourceAggregation;
using ITest.SeedWork;
using MongoDB.Driver;
using Newtonsoft.Json;
using Reporting.Domain.ReportAggregate;
using Reporting.Domain.StatementAggregate;
using Reporting.Domain.TransactionAggregate;
using Reporting.Domain.ValueObjects;
using Xunit;

namespace Reporting.ITest.Scenarios;

public class AggregateTransactionsScenarios: IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    const string Url = "api/v1/reports/aggregate";
    private const string ConfigIdStr = "3de74b1c-36db-4b19-9694-e6a213252982";
    private const int Year = 2023;
    private const int Month = 3;
    
    private readonly HttpClient _client;
    private readonly MongoDbTestInstance _dbTestInstance;
    private readonly IMongoCollection<Statement> _statementCollection;
    private readonly IMongoCollection<Transaction> _transactionCollection;
    private readonly IMongoCollection<Source> _sourceCollection;
    private readonly IMongoCollection<Report> _reportCollection;

    public AggregateTransactionsScenarios(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _dbTestInstance = new MongoDbTestInstance();
        _statementCollection = _dbTestInstance.GetCollection<Statement>();
        _transactionCollection = _dbTestInstance.GetCollection<Transaction>();
        _sourceCollection = _dbTestInstance.GetCollection<Source>();
        _reportCollection = _dbTestInstance.GetCollection<Report>();
    }

    public void Dispose()
    { 
        _dbTestInstance.Client.DropDatabase(MongoDbTestInstance.DatabaseName);
    }
    
    [Fact]
    public async Task Post_Success()
    {
        await AddSourceSeed();
        await AddTransactionsSeed();
        
        var request = new StringContent(GetRequestBody(), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(Url, request);

        response.EnsureSuccessStatusCode();
        
        var statement = await _statementCollection.Find(s => s.ConfigId == Guid.Parse(ConfigIdStr)).FirstOrDefaultAsync();
        var report = await _reportCollection.Find(r => r.ConfigId == Guid.Parse(ConfigIdStr)).FirstOrDefaultAsync();
        
        Assert.True(statement is not null);
        Assert.True(report is not null);
    }

    [Fact]
    public async Task Post_StatementAlreadyExists_Failed()
    {
        await AddSourceSeed();
        await AddTransactionsSeed();
        
        var request = new StringContent(GetRequestBody(), Encoding.UTF8, "application/json");
        
        // Send the same request twice
        // Simulate the case that the transaction exists in the database
        await _client.PostAsync(Url, request);
        var response = await _client.PostAsync(Url, request);
        
        Assert.True(response.StatusCode == HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task Post_SourceConfiguringNotFound_Failed()
    {
        await AddTransactionsSeed();
        
        var request = new StringContent(GetRequestBody(), Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync(Url, request);
        
        Assert.True(response.StatusCode == HttpStatusCode.UnprocessableEntity);
    }
    
    private async Task AddSourceSeed()
    {
        var source = new Source
        {
            ConfigId = Guid.Parse(ConfigIdStr),
            Categories = new[]
            {
                new Category
                {
                    Keywords = new List<string>()
                }
            }
        };
        
        await _sourceCollection.InsertOneAsync(source);
    }

    private async Task AddTransactionsSeed()
    {
        var transactionRow1 = new TransactionRow
        {
            Label = "\"Dummy transaction CB*1228\"",
            Amount = -6.15m,
            Currency = "EUR"
        };
        
        var transactionRow2 = new TransactionRow
        {
            Label = "CARTE X0426 30/05 LIDL     1482",
            Amount = -11.87m,
            Currency = "EUR"
        };

        var transaction1 = new Transaction
        {
            ConfigId = Guid.Parse(ConfigIdStr),
            Year = Year,
            Month = Month,
            Rows = new[] { transactionRow1 }
        };

        var transaction2 = new Transaction
        {
            ConfigId = Guid.Parse(ConfigIdStr),
            Year = Year,
            Month = Month,
            Rows = new[] { transactionRow2 }
        };
        
        await _transactionCollection.InsertManyAsync(new[] { transaction1, transaction2 });
    }

    private static string GetRequestBody()
    {
        var command = new
        {
            ConfigId = ConfigIdStr,
            Year = Year.ToString(),
            Month = Month.ToString()
        };
        return JsonConvert.SerializeObject(command);
    }

}