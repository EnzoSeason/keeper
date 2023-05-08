using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Reporting.API.Commands.UploadTransactionFile;

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
