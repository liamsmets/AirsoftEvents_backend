using Microsoft.AspNetCore.Mvc;
using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AirsoftEvents.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [Authorize(Policy = "ApiWritePolicy")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequestContract userToCreate)
    {
        try
        {
            var createdUser = await _service.RegisterUserAsync(userToCreate);
            
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Policy = "ApiReadPolicy")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id)
    {
        var user = await _service.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}