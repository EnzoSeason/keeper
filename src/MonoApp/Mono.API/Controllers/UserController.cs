using Microsoft.AspNetCore.Mvc;
using Mono.API.Models;
using Mono.API.Services;

namespace Mono.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly UsersService _usersService;
    
    public UserController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _usersService.GetUsers();
        
        return new ActionResult<IEnumerable<User>>(users);
    }
    
    [HttpGet("{uid:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUser(Guid uid)
    {
        var user = await _usersService.GetUser(uid);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<User>> CreateUser([FromQuery] string username)
    {
        var newUser = new User(Guid.NewGuid(), username);
        await _usersService.CreateUser(newUser);
        
        return CreatedAtAction(nameof(CreateUser), null, newUser);
    }
}