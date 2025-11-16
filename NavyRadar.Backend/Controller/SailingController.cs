using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Sailing;
using NavyRadar.Shared.Domain.Sail;

namespace NavyRadar.Backend.Controller;

[ApiController]
[Authorize(Roles = "Captain")]
[Route("api/[controller]")]
public class SailingController(ISailingService sailingService) : ControllerBase
{
    private (bool, int) TryGetAccountId()
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.Sid);
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out var accountId))
        {
            return (false, 0);
        }

        return (true, accountId);
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(SailWithName), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCaptainActiveSail()
    {
        var (isSuccess, accountId) = TryGetAccountId();
        if (!isSuccess) return Unauthorized();

        var sail = await sailingService.GetActiveSailByCaptainIdAsync(accountId);
        if (sail == null)
        {
            return NotFound();
        }

        return Ok(sail);
    }

    [HttpPost("position")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPositionHistory([FromBody] AddPositionRequest request)
    {
        var (isSuccess, accountId) = TryGetAccountId();
        if (!isSuccess) return Unauthorized();

        var added = await sailingService.AddPositionHistoryAsync(accountId, request);
        if (!added)
        {
            return NotFound(new { Message = "No active sail found to add position to." });
        }

        return Created();
    }

    [HttpPut("active/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteActiveSail()
    {
        var (isSuccess, accountId) = TryGetAccountId();
        if (!isSuccess) return Unauthorized();

        var completed = await sailingService.CompleteActiveSailAsync(accountId);
        if (!completed)
        {
            return NotFound(new { Message = "No active sail found to complete." });
        }

        return Ok(new { Message = "Sail completed successfully." });
    }

    [HttpPut("active/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateActiveSailStatus([FromBody] UpdateSailStatusRequest request)
    {
        var (isSuccess, accountId) = TryGetAccountId();
        if (!isSuccess) return Unauthorized();

        var updated = await sailingService.UpdateSailStatusAsync(accountId, request.Status);

        if (!updated)
        {
            return NotFound(new { Message = "No active sail found or status update was invalid." });
        }

        return Ok(new { Message = "Sail status updated successfully." });
    }
}