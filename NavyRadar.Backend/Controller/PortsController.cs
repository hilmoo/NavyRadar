using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class PortsController(IPortService portService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<Port>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPorts()
    {
        var ports = await portService.GetAllAsync();
        return Ok(ports);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Port), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPortById(int id)
    {
        var port = await portService.GetByIdAsync(id);
        if (port == null)
        {
            return NotFound();
        }

        return Ok(port);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Port), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreatePort([FromBody] Port port)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdPort = await portService.CreateAsync(port);
        if (createdPort == null)
        {
            return BadRequest("Could not create port.");
        }

        return CreatedAtAction(nameof(GetPortById), new { id = createdPort.Id }, createdPort);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Port), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePort(int id, [FromBody] Port port)
    {
        if (id != port.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedPort = await portService.UpdateAsync(id, port);
        if (updatedPort == null)
        {
            return NotFound();
        }

        return Ok(updatedPort);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePort(int id)
    {
        var deleted = await portService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}