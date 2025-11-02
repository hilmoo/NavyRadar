using Backend.IService;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class SailsController(ISailService sailService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllSails()
    {
        var sails = await sailService.GetAllAsync();
        return Ok(sails);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSailById(int id)
    {
        var sail = await sailService.GetByIdAsync(id);
        if (sail == null)
        {
            return NotFound();
        }

        return Ok(sail);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSail([FromBody] Sail sail)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdSail = await sailService.CreateAsync(sail);
        if (createdSail == null)
        {
            return BadRequest("Could not create sail.");
        }

        return CreatedAtAction(nameof(GetSailById), new { id = createdSail.Id }, createdSail);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSail(int id, [FromBody] Sail sail)
    {
        if (id != sail.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedSail = await sailService.UpdateAsync(id, sail);
        if (updatedSail == null)
        {
            return NotFound();
        }

        return Ok(updatedSail);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSail(int id)
    {
        var deleted = await sailService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}