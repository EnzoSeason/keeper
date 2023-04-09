using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonoAPI.Models;

namespace MonoAPI.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserContext _context;

    public UserController(UserContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<User>> CreateUser([FromBody] User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(CreateUser), new { id = user.Id }, user);
    }
}