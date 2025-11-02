using Backend.IService;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;


namespace Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class PositionHistoriesController(IPositionHistoryService positionHistoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPositionHistories()
    {
        var positionHistories = await positionHistoryService.GetAllAsync();
        return Ok(positionHistories);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPositionHistoryById(int id)
    {
        var positionHistory = await positionHistoryService.GetByIdAsync(id);
        if (positionHistory == null)
        {
            return NotFound();
        }

        return Ok(positionHistory);
    }

    [HttpGet("sail/{sailId:int}")]
    public async Task<IActionResult> GetPositionHistoriesBySailId(int sailId)
    {
        var positionHistories = await positionHistoryService.GetBySailIdAsync(sailId);
        return Ok(positionHistories);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePositionHistory(
        [FromBody] PositionHistory positionHistory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdPositionHistory = await positionHistoryService.CreateAsync(positionHistory);
        if (createdPositionHistory == null)
        {
            return BadRequest("Could not create position history.");
        }

        return CreatedAtAction(nameof(GetPositionHistoryById), new { id = createdPositionHistory.Id },
            createdPositionHistory);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePositionHistory(int id, [FromBody] PositionHistory positionHistory)
    {
        if (id != positionHistory.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedPositionHistory = await positionHistoryService.UpdateAsync(id, positionHistory);
        if (updatedPositionHistory == null)
        {
            return NotFound();
        }

        return Ok(updatedPositionHistory);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePositionHistory(int id)
    {
        var deleted = await positionHistoryService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}