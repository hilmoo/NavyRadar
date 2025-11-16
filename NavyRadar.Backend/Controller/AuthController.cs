using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Auth;

namespace NavyRadar.Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AccountWithAuth), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] PayloadRegister payloadRegister)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var account = await authService.RegisterAsync(payloadRegister);

        if (account == null)
        {
            return BadRequest("Registration failed. Username or email may already be in use.");
        }

        return Created($"/api/accounts/{account.UserAccount.Id}", account);
    }

    [HttpPost("signin")]
    [ProducesResponseType(typeof(AccountWithAuth), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] PayloadLogin payloadLogin)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var account = await authService.SignInService(payloadLogin);

        if (account == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(account);
    }
}