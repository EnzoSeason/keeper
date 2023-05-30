using Domain.SeedWork;
using Reporting.Domain.StatementAggregate;

namespace Reporting.Domain.ReportAggregate;

public interface IReportRepository : IRepository<Report>
{
    Task Analyze(Statement statement);
}