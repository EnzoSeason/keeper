namespace Reporting.API.Commands;

public record UploadTransactionFileCommand
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