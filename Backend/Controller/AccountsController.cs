using Backend.IService;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await accountService.GetAllAsync();
        return Ok(accounts);
    }

    [HttpGet("{id:int}")]
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
    public async Task<IActionResult> CreateAccount([FromBody] Account account)
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
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] Account account)
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

    [HttpDelete("{id:int}")]
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