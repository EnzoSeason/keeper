using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Reporting.API.Utils;
using Reporting.Domain.AggregatesModel.TransactionAggregate;
using Reporting.Domain.AggregatesModel.ValueObjects;
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

    public async Task<bool> Handle(UploadTransactionFileCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        var origin = new Origin
        {
            Type = OriginType.File,
            Description = request.File.FileName
        };

        if (await _repository.IsFound(request.ConfigId, request.Year, request.Month, origin))
        {
            return false;
        }

        // TODO: Replace the hardcoded csv config by the configuration service
        
        List<TransactionRow> transactionRows;
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
        
            transactionRows = rows;
        }
        
        var transaction = new Transaction
        {
            ConfigId = request.ConfigId,
            Year = request.Year,
            Month = request.Month,
            Version = new DateTimeOffset(_clock.Now).ToUnixTimeMilliseconds(),
            Origin = origin,
            Rows = transactionRows
        };

        if (!DomainModelValidator<Transaction>.TryValidate(transaction, out var validationResults))
        {
            // TODO: Log the validation results
            return false;
        }

        try
        {
            await _repository.InsertOne(transaction);
        }
        catch
        {
            // TODO: Log the error
            return false;
        }

        return true;
    }
}