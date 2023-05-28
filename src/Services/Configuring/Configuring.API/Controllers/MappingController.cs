using Configuring.Domain.SourceAggregation;
using Microsoft.AspNetCore.Mvc;

namespace Configuring.API.Controllers;


/// <summary>
/// Configuring how to map the transaction rows into different categories.
/// It's based on the source of transactions (e.g. SG (Bank))
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class MappingController: ControllerBase
{
    private readonly ISourceRepository _sourceRepository;

    public MappingController(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(Source newSource)
    {
        if (await _sourceRepository.IsFound(newSource.ConfigId))
        {
            return BadRequest("The Source Configuring exists. Please update it.");
        }
        
        await _sourceRepository.InsertOne(newSource);
        
        return CreatedAtAction(nameof(Create), new { configId = newSource.ConfigId }, newSource);
    }
}