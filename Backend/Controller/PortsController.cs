using Backend.IService;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class PortsController(IPortService portService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPorts()
    {
        var ports = await portService.GetAllAsync();
        return Ok(ports);
    }

    [HttpGet("{id:int}")]
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