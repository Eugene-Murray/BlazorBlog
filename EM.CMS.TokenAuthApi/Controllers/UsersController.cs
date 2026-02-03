using Microsoft.AspNetCore.Mvc;
using TokenAuthApi.Models;
using TokenAuthApi.Services;

namespace TokenAuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly CosmosUserService _userService;

    public UsersController(CosmosUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] AppUser user)
    {
        var existing = await _userService.GetUserAsync(user.Username);
        if (existing != null)
            return Conflict("Username already exists.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        user.Roles = user.Roles ?? new List<string> { "User" };

        await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { username = user.Username }, new { user.Id, user.Username, user.Roles });
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await _userService.GetUserAsync(username);
        if (user == null)
            return NotFound();

        return Ok(new { user.Id, user.Username, user.Roles });
    }
}