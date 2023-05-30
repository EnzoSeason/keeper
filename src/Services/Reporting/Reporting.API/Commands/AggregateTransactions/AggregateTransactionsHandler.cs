using MediatR;
using Reporting.Domain.ReportAggregate;
using Reporting.Domain.StatementAggregate;

namespace Reporting.API.Commands.AggregateTransactions;

public class AggregateTransactionsHandler : IRequestHandler<AggregateTransactionsCommand, bool>
{
    private readonly IStatementRepository _statementRepository;
    private readonly IReportRepository _reportRepository;

    public AggregateTransactionsHandler(IStatementRepository statementRepository, IReportRepository reportRepository)
    {
        _statementRepository = statementRepository;
        _reportRepository = reportRepository;
    }

    public async Task<bool> Handle(AggregateTransactionsCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }
        
        if (await _statementRepository.IsFound(request.ConfigId, request.Year, request.Month))
        {
            return false;
        }

        try
        {
            var statement =
                await _statementRepository.AggregateTransactions(request.ConfigId, request.Year, request.Month);
            await _reportRepository.Analyze(statement);
        }
        catch
        {
            // TODO: Log the error
            return false;
        }

        return true;
    }
}