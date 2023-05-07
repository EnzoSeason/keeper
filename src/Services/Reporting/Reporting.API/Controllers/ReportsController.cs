using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporting.API.Commands;

namespace Reporting.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReportsController: ControllerBase
{
    private readonly IMediator _mediator;
    
    public ReportsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Upload([FromForm] UploadTransactionFileCommand command)
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

        var commandResult = await _mediator.Send(command);

        if (!commandResult)
        {
            return UnprocessableEntity("Upload file command failed.");
        }

        return NoContent();
    }
}