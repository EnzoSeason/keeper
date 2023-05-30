using System.ComponentModel;
using MediatR;

namespace Reporting.API.Commands.AggregateTransactions;

/// <summary>
/// Create a statement by aggregating the transactions
/// </summary>
public class AggregateTransactionsCommand: IRequest<bool>
{
    /// <summary>
    /// The ID of the configuration used for creating the report
    /// </summary>
    [DefaultValue("3de74b1c-36db-4b19-9694-e6a213252982")]
    public Guid ConfigId { get; init; }
    
    /// <summary>
    /// The year when the transactions happen.
    /// For example, 2023.
    /// </summary>
    [DefaultValue("2023")]
    public int Year { get; init; }
    
    /// <summary>
    /// The month when the transactions happen.
    /// For example, March is 3.
    /// </summary>
    [DefaultValue("3")]
    public int Month { get; init; }
}