using System.ComponentModel.DataAnnotations;

namespace Reporting.Domain.ValueObjects;

public static class TransactionRowsValidation
{
    public static IEnumerable<ValidationResult> Validate(this IEnumerable<TransactionRow> rows, int year, int month)
    {
        var results = new List<ValidationResult>();

        if (!rows.Any())
        {
            results.Add(new ValidationResult("Rows must contain data.", new[] { nameof(rows) }));
            return results;
        }

        var rowsYears = rows.Select(row => row.Date.Year).Distinct().ToList();
        var rowsMonths = rows.Select(row => row.Date.Month).Distinct().ToList();

        if (rowsYears.Count != 1 || rowsMonths.Count != 1)
        {
            results.Add(
                new ValidationResult("Rows must be the transactions of the same month in the same year.",
                    new[] { nameof(rows) }));
            return results;
        }
        
        if (rowsYears.First() != year)
        {
            results.Add(
                new ValidationResult("The transaction year in rows doesn't match to the one in metadata.",
                    new[] { nameof(rows) }));
        }

        if (rowsMonths.First() != month)
        {
            results.Add(
                new ValidationResult("The transaction month in rows doesn't match to the one in metadata.",
                    new[] { nameof(rows) }));
        }

        return results;
    }
}