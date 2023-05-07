using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Reporting.Domain.SeedWork;
using Reporting.Domain.TransactionModels;
using Reporting.Infrastructure.Repositories;

namespace Reporting.API.Commands;

public class UploadTransactionFileHandler : IRequestHandler<UploadTransactionFileCommand, bool>
{
    private readonly ITransactionRepository _repository;

    public UploadTransactionFileHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(UploadTransactionFileCommand request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction
        {
            ConfigId = request.ConfigId,
            FileDate = request.FileDate
        };
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
            _repository.InsertTransaction(Transaction.ToEntity(transaction));
        }
        catch
        {
            // TODO: Log the error
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}