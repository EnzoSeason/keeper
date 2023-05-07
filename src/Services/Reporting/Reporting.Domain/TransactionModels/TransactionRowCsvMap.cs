using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Reporting.Domain.SeedWork;

namespace Reporting.Domain.TransactionModels;

public sealed class TransactionRowCsvMap: ClassMap<TransactionRow>
{
    public TransactionRowCsvMap()
    {
        Map(m => m.Date)
            .TypeConverter<DateTimeConverter>()
            .TypeConverterOption.Format("dd/MM/yyyy");
        
        Map(m => m.Label);
        
        Map(m => m.Amount)
            .TypeConverter<CustomDoubleConverter>();
        
        Map(m => m.Currency);
    }
}