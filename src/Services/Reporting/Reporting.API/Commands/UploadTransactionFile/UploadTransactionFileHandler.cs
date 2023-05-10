using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Reporting.API.Utils;
using Reporting.Domain.AggregatesModel.TransactionAggregate;
using Reporting.Domain.SeedWork;

namespace Reporting.API.Commands.UploadTransactionFile;

public class UploadTransactionFileHandler : IRequestHandler<UploadTransactionFileCommand, bool>
{
    private readonly ITransactionRepository _repository;
    private readonly IClock _clock;

    public UploadTransactionFileHandler(ITransactionRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public Task<bool> Handle(UploadTransactionFileCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(false);
        }
        
        var transaction = new Transaction
        {
            ConfigId = request.ConfigId,
            Year = request.Year,
            Month = request.Month,
            Version = _clock.Now.Millisecond,
            Origin = new Origin
            {
                Type = OriginType.File,
                Description = request.File.FileName
            }
        };
        
        // TODO: Replace the hardcoded csv config by the configuration service
        
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };

        using (var reader = new StreamReader(request.File.OpenReadStream()))
        using (var csv = new CsvReader(reader, csvConfig))
        {
            csv.Context.RegisterClassMap<TransactionRowCsvMap>();
            
            var records = csv.GetRecords<TransactionRow>();

            var rows = records
                .Where(record => record != null)
                .Where(DomainModelValidator<TransactionRow>.TryValidate)
                .ToList();
        
            transaction.Rows = rows;
        }

        if (!DomainModelValidator<Transaction>.TryValidate(transaction, out var validationResults))
        {
            // TODO: Log the validation results
            return Task.FromResult(false);
        }

        try
        {
            _repository.InsertOne(transaction);
        }
        catch
        {
            // TODO: Log the error
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}