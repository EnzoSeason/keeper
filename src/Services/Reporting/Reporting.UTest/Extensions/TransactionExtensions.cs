using Reporting.Domain.AggregatesModel.TransactionAggregate;

namespace Reporting.UTest.Extensions;

public static class TransactionExtensions
{
    public static bool TransactionEquals(this Transaction self, Transaction other)
    {
        return self.Id == other.Id &&
               self.ConfigId.Equals(other.ConfigId) &&
               self.Year == other.Year &&
               self.Month == other.Month &&
               self.Version == other.Version &&
               self.Origin.Equals(other.Origin) &&
               self.Rows.SequenceEqual(other.Rows);
    }
}