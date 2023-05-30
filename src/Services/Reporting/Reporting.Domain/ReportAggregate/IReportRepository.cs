using Reporting.Domain.StatementAggregate;

namespace Reporting.Domain.ReportAggregate;

public interface IReportRepository
{
    Task Build(Statement statement);
}