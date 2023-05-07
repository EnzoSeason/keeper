using MediatR;

namespace Reporting.API.Commands;

// DDD and CQRS patterns comment: Note that it is recommended to implement immutable Commands
// In this case, its immutability is achieved by the record without setters
public record UploadTransactionFileCommand: IRequest<bool>
{
    /// <summary>
    /// The ID of the configuration used for creating the report
    /// </summary>
    public Guid ConfigId { get; init; }
    
    /// <summary>
    /// The datetime when the transactions happen. Measuring precisely in <b>months</b>.
    /// e.g. The uploaded file contains all the transactions of April, 2023.
    /// </summary>
    public DateTime FileDate { get; init; }
    
    /// <summary>
    /// The transaction file in CSV format
    /// </summary>
    public IFormFile File { get; init; }
}