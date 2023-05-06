using Microsoft.AspNetCore.Mvc;
using Reporting.API.Commands;

namespace Reporting.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReportsController: ControllerBase
{
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult Upload([FromForm] UploadTransactionFileCommand command)
    {
        // TODO: check if config id exists in database

        if (command.File.Length == 0)
        {
            return BadRequest("File should not be empty.");
        }

        if (Path.GetExtension(command.File.FileName).ToLower() != ".csv")
        {
            return BadRequest("Only the CSV file is accepted.");
        }

        return NoContent();
    }
}