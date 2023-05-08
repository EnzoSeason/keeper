using System.ComponentModel;
using MediatR;

namespace Reporting.API.Commands.UploadTransactionFile;

// DDD and CQRS patterns comment: Note that it is recommended to implement immutable Commands
// In this case, its immutability is achieved by the record without setters
public record UploadTransactionFileCommand: IRequest<bool>
{
    /// <summary>
    /// The ID of the configuration used for creating the report
    /// </summary>
    public Guid ConfigId { get; init; }
    
    /// <summary>
    /// The month when the transactions happen.
    /// For example, March is 3.
    /// </summary>
    [DefaultValue("3")]
    public int Month { get; init; }
    
    /// <summary>
    /// The transaction file in CSV format
    /// </summary>
    public IFormFile File { get; init; } = null!;
}