using Reporting.Domain.ReportAggregate;
using Reporting.Domain.StatementAggregate;

namespace Reporting.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    public Task Analyze(Statement statement)
    {
        // TODO: implement the logic
        return Task.CompletedTask;
    }
}