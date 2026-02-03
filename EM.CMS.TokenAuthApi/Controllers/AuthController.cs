using Microsoft.AspNetCore.Mvc;
using TokenAuthApi.Models;
using TokenAuthApi.Services;

namespace TokenAuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CosmosUserService _userService;
    private readonly TokenService _tokenService;

    public AuthController(CosmosUserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.GetUserAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized();

        var token = _tokenService.GenerateToken(user);
        return Ok(new { token });
    }
}