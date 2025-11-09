using Backend.IService;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain;
using Shared.Models;

namespace Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AccWithAuth), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var account = await authService.RegisterAsync(registerDto);

        if (account == null)
        {
            return BadRequest("Registration failed. Username or email may already be in use.");
        }

        return Created($"/api/accounts/{account.UserAccount.Id}", account);
    }

    [HttpPost("signin")]
    [ProducesResponseType(typeof(AccWithAuth), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var account = await authService.SignInService(loginDto);

        if (account == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(account);
    }
}