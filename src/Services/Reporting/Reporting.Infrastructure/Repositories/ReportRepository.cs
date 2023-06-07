using Configuring.Domain.SourceAggregation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Reporting.Domain.ReportAggregate;
using Reporting.Domain.StatementAggregate;
using Reporting.Infrastructure.Settings;

namespace Reporting.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly IMongoCollection<Report> _reportCollection;
    private readonly IMongoCollection<Source> _sourceCollection;

    public ReportRepository(IOptions<ReportingDbSettings> reportingDbSettings)
    {
        var reportingDatabase = ReportingDbHelper.GetDatabase(reportingDbSettings);
        
        _reportCollection = reportingDatabase.GetCollection<Report>(reportingDbSettings.Value.ReportCollectionName);
        _sourceCollection = reportingDatabase.GetCollection<Source>(reportingDbSettings.Value.SourceCollectionName);
    }

    public async Task Analyze(Statement statement)
    {
        var source = await _sourceCollection.Find(s => s.ConfigId == statement.ConfigId).FirstOrDefaultAsync();

        if (source == null)
        {
            throw new Exception("Source Configuring is not found with the given config Id: " +
                                statement.ConfigId.ToString("N"));
        }
        
        var report = Report.Analyze(source, statement);
        
        await _reportCollection.InsertOneAsync(report);
    }
}