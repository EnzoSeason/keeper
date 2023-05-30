using MediatR;
using Reporting.Domain.StatementAggregate;

namespace Reporting.API.Commands.AggregateTransactions;

public class AggregateTransactionsHandler : IRequestHandler<AggregateTransactionsCommand, bool>
{
    private readonly IStatementRepository _repository;

    public AggregateTransactionsHandler(IStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(AggregateTransactionsCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }
        
        if (await _repository.IsFound(request.ConfigId, request.Year, request.Month))
        {
            return false;
        }

        try
        {
            await _repository.AggregateTransactions(request.ConfigId, request.Year, request.Month);
        }
        catch
        {
            // TODO: Log the error
            return false;
        }

        return true;
    }
}