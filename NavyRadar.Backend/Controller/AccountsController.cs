using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Account;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<AccountBase>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await accountService.GetAllAsync();
        return Ok(accounts);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AccountBase), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAccountById(int id)
    {
        var account = await accountService.GetByIdAsync(id);
        if (account == null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AccountBase), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAccount([FromBody] AccountPassword account)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdAccount = await accountService.CreateAsync(account);
        if (createdAccount == null)
        {
            return BadRequest("Could not create account.");
        }

        return CreatedAtAction(nameof(GetAccountById), new { id = createdAccount.Id }, createdAccount);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AccountBase), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccount account)
    {
        if (id != account.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedAccount = await accountService.UpdateAsync(id, account);
        if (updatedAccount == null)
        {
            return NotFound();
        }

        return Ok(updatedAccount);
    }

    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(AccountBase), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateAccount account)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.Sid);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid user ID in token");
        }

        Debug.WriteLine(account.Id);
        Debug.WriteLine(account.Email);
        Debug.WriteLine(userId);
        if (account.Id != userId)
        {
            return BadRequest("You can only update your own profile.");
        }

        var updatedAccount = await accountService.UpdateAsync(userId, account);
        if (updatedAccount == null)
        {
            return NotFound();
        }

        return Ok(updatedAccount);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var deleted = await accountService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}