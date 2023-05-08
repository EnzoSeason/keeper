using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Reporting.Domain.AggregatesModel.TransactionAggregate;

namespace Reporting.API.Commands.UploadTransactionFile;

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

public class CustomDoubleConverter : DoubleConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        text = text?.Replace(",", ".");
        
        if (double.TryParse(text, out var result))
        {
            return result;
        }

        return base.ConvertFromString(text, row, memberMapData);
    }
}