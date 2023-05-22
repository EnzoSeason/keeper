using System.Net;
using MongoDB.Driver;
using Reporting.Domain.TransactionAggregate;
using Xunit;

namespace Reporting.ITest.Scenarios;

public class UploadTransactionFileScenarios : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    const string Url = "api/v1/reports/upload";
    
    private readonly HttpClient _client;
    private readonly MongoDbTestInstance _dbTestInstance;
    private readonly IMongoCollection<Transaction> _transactionCollection;

    public UploadTransactionFileScenarios(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _dbTestInstance = new MongoDbTestInstance();
        _transactionCollection = _dbTestInstance.GetCollection<Transaction>();
    }

    public void Dispose()
    { 
        _dbTestInstance.Client.DropDatabase(MongoDbTestInstance.DatabaseName);
    }

    [Fact]
    public async Task Post_Success()
    {
        var formData = GetRequest();

        var response = await _client.PostAsync(Url, formData);

        response.EnsureSuccessStatusCode();
        var query = await _transactionCollection.FindAsync(_ => true);
        var result = await query.ToListAsync();
        Assert.True(result.Count > 0);
    }

    [Fact]
    public async Task Post_FileAlreadyExists_Failed()
    {
        var formData = GetRequest();
        
        // Send the same request twice
        // Simulate the case that the transaction exists in the database
        await _client.PostAsync(Url, formData);
        var response = await _client.PostAsync(Url, formData);
        
        Assert.True(response.StatusCode == HttpStatusCode.UnprocessableEntity);
    }
    
    private static MultipartFormDataContent GetRequest()
    {
        const string rows = @"
Date;Label;Amount;Currency;
29/03/2023;FRANPRIX;-6,15;EUR;
29/03/2023;PYMT;16,08;EUR;
";
        
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("0b38769b-c94b-4b85-8532-91adb844b6eb"), "ConfigId");
        formData.Add(new StringContent("2023"), "Year");
        formData.Add(new StringContent("3"), "Month");
        formData.Add(new StreamContent(GetStream(rows)), "File", "test.csv");
        return formData;
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