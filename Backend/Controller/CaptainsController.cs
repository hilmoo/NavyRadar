using Backend.IService;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class CaptainsController(ICaptainService captainService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCaptains()
    {
        var captains = await captainService.GetAllAsync();
        return Ok(captains);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCaptainById(int id)
    {
        var captain = await captainService.GetByIdAsync(id);
        if (captain == null)
        {
            return NotFound();
        }

        return Ok(captain);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCaptain([FromBody] Captain captain)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdCaptain = await captainService.CreateAsync(captain);
        if (createdCaptain == null)
        {
            return BadRequest("Could not create captain.");
        }

        return CreatedAtAction(nameof(GetCaptainById), new { id = createdCaptain.Id }, createdCaptain);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCaptain(int id, [FromBody] Captain captain)
    {
        if (id != captain.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedCaptain = await captainService.UpdateAsync(id, captain);
        if (updatedCaptain == null)
        {
            return NotFound();
        }

        return Ok(updatedCaptain);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCaptain(int id)
    {
        var deleted = await captainService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}