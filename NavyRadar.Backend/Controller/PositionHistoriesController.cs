using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Entities;


namespace NavyRadar.Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class PositionHistoriesController(IPositionHistoryService positionHistoryService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<PositionHistory>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllPositionHistories()
    {
        var positionHistories = await positionHistoryService.GetAllAsync();
        return Ok(positionHistories);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PositionHistory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<PositionHistory>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPositionHistoriesBySailId(int sailId)
    {
        var positionHistories = await positionHistoryService.GetBySailIdAsync(sailId);
        return Ok(positionHistories);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PositionHistory), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PositionHistory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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