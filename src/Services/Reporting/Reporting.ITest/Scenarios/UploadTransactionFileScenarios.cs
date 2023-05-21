using System.Net;
using Reporting.Domain.AggregatesModel.TransactionAggregate;
using Xunit;

namespace Reporting.ITest.Scenarios;

public class UploadTransactionFileScenarios : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly MongoDbTestInstance _dbTestInstance;

    public UploadTransactionFileScenarios(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _dbTestInstance = new MongoDbTestInstance();
    }

    public void Dispose()
    { 
        _dbTestInstance.Client.DropDatabase(MongoDbTestInstance.DatabaseName);
    }

    [Fact]
    public async Task Post_UploadTransactionFile_Success()
    {
        const string url = "api/v1/reports/upload";
        var formData = GetRequest();

        var response = await _client.PostAsync(url, formData);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Post_UploadTransactionFile_FileAlreadyExists_Failed()
    {
        const string url = "api/v1/reports/upload";
        var formData = GetRequest();
        
        // Send the same request twice
        // Simulate the case that the transaction exists in the database
        await _client.PostAsync(url, formData);
        var response = await _client.PostAsync(url, formData);
        
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