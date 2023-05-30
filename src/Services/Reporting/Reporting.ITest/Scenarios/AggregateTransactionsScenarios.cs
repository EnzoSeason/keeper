using System.Net;
using System.Text;
using ITest.SeedWork;
using MongoDB.Driver;
using Newtonsoft.Json;
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

    public AggregateTransactionsScenarios(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _dbTestInstance = new MongoDbTestInstance();
        _statementCollection = _dbTestInstance.GetCollection<Statement>();
        _transactionCollection = _dbTestInstance.GetCollection<Transaction>();
        
        AddTransactionsSeed();
    }
    
    public void Dispose()
    { 
        _dbTestInstance.Client.DropDatabase(MongoDbTestInstance.DatabaseName);
    }
    
    [Fact]
    public async Task Post_Success()
    {
        var request = new StringContent(GetRequestBody(), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(Url, request);

        response.EnsureSuccessStatusCode();
        var result = await _statementCollection.FindAsync(_ => true);
        var resultList = await result.ToListAsync();
        Assert.True(resultList.Count == 1);
    }
    
    [Fact]
    public async Task Post_StatementAlreadyExists_Failed()
    {
        var request = new StringContent(GetRequestBody(), Encoding.UTF8, "application/json");
        
        // Send the same request twice
        // Simulate the case that the transaction exists in the database
        await _client.PostAsync(Url, request);
        var response = await _client.PostAsync(Url, request);
        
        Assert.True(response.StatusCode == HttpStatusCode.UnprocessableEntity);
    }

    private void AddTransactionsSeed()
    {
        var transactionRow = new TransactionRow();

        var transaction1 = new Transaction
        {
            ConfigId = Guid.Parse(ConfigIdStr),
            Year = Year,
            Month = Month,
            Rows = new[] { transactionRow }
        };

        var transaction2 = new Transaction
        {
            ConfigId = Guid.Parse(ConfigIdStr),
            Year = Year,
            Month = Month,
            Rows = new[] { transactionRow }
        };
        
        _transactionCollection.InsertMany(new[] { transaction1, transaction2 });
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