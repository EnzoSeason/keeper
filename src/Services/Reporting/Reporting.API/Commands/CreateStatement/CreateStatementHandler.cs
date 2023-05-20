using MediatR;
using Reporting.Domain.AggregatesModel.StatementAggregate;

namespace Reporting.API.Commands.CreateStatement;

public class CreateStatementHandler : IRequestHandler<CreateStatementCommand, bool>
{
    private readonly IStatementRepository _repository;

    public CreateStatementHandler(IStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CreateStatementCommand request, CancellationToken cancellationToken)
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