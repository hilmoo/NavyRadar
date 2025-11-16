using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class ShipController(IShipService shipService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<Ship>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllShips()
    {
        var ships = await shipService.GetAllAsync();
        return Ok(ships);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Ship), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShipById(int id)
    {
        var ship = await shipService.GetByIdAsync(id);
        if (ship == null)
        {
            return NotFound();
        }

        return Ok(ship);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Ship), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateShip([FromBody] Ship ship)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdShip = await shipService.CreateAsync(ship);
        if (createdShip == null)
        {
            return BadRequest("Could not create ship.");
        }

        return CreatedAtAction(nameof(GetShipById), new { id = createdShip.Id }, createdShip);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Ship), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShip(int id, [FromBody] Ship ship)
    {
        if (id != ship.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedShip = await shipService.UpdateAsync(id, ship);
        if (updatedShip == null)
        {
            return NotFound();
        }

        return Ok(updatedShip);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteShip(int id)
    {
        var deleted = await shipService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}