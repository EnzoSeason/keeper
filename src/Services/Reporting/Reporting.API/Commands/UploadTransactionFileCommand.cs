namespace Reporting.API.Commands;

public record UploadTransactionFileCommand
{
    /// <summary>
    /// The ID of user who owns the transaction file
    /// </summary>
    public Guid UserId { get; init; }
    
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