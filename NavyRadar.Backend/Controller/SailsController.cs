using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain;
using NavyRadar.Shared.Models;

namespace NavyRadar.Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class SailsController(ISailService sailService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<Sail>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSails()
    {
        var sails = await sailService.GetAllAsync();
        return Ok(sails);
    }

    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<ActiveSailPosition>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllActiveSailPosition()
    {
        var sails = await sailService.GetAllActiveSailPositionAsync();
        return Ok(sails);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Sail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Sail), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Sail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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