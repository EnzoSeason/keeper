using MediatR;

namespace Reporting.API.Commands;

public class UploadTransactionFileHandler : IRequestHandler<UploadTransactionFileCommand, bool>
{
    public Task<bool> Handle(UploadTransactionFileCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}