using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Reporting.Domain.ValueObjects;

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
            .TypeConverter<CustomDecimalConverter>();
        
        Map(m => m.Currency);
    }
}

public class CustomDecimalConverter : DecimalConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (decimal.TryParse(text, new NumberFormatInfo { NumberDecimalSeparator = ","}, out var result))
        {
            return result;
        }

        return base.ConvertFromString(text, row, memberMapData);
    }
}