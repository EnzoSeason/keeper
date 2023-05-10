using Xunit;

namespace Reporting.ITest.Controllers;

public class ReportingScenarios : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly MongoDbTestInstance _dbTestInstance;

    public ReportingScenarios(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _dbTestInstance = new MongoDbTestInstance();
    }

    public void Dispose()
    { 
        _dbTestInstance.Client.DropDatabase(MongoDbTestInstance.DatabaseName);
    }

    [Fact]
    public async Task Post_UploadTransactionFile()
    {
        const string url = "api/v1/reports/upload";
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

        var response = await _client.PostAsync(url, formData);

        response.EnsureSuccessStatusCode();
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