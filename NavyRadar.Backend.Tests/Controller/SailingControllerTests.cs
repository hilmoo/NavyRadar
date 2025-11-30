using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.Controller;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Sail;
using NavyRadar.Shared.Domain.Sailing;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Tests.Controller;

public class SailingControllerTests
{
    private readonly Mock<ISailingService> _mockSailingService;
    private readonly SailingController _controller;

    public SailingControllerTests()
    {
        _mockSailingService = new Mock<ISailingService>();
        _controller = new SailingController(_mockSailingService.Object);
    }

    private void SetupCaptainClaims(int captainId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, captainId.ToString()),
            new(ClaimTypes.Role, "Captain")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetCaptainActiveSail_WhenSailExists_ShouldReturnOkWithSail()
    {
        SetupCaptainClaims(1);

        var sail = new SailWithName
        {
            Id = 1,
            CaptainId = 1,
            ShipId = 1,
            Status = SailStatus.Sailing,
            DepartureTime = DateTime.UtcNow,
            OriginPortId = 1,
            DestinationPortId = 2
        };

        _mockSailingService.Setup(s => s.GetActiveSailByCaptainIdAsync(1))
            .ReturnsAsync(sail);

        var result = await _controller.GetCaptainActiveSail();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSail = Assert.IsType<SailWithName>(okResult.Value);
        Assert.Equal(1, returnedSail.Id);
    }

    [Fact]
    public async Task GetCaptainActiveSail_WhenNoActiveSail_ShouldReturnNotFound()
    {
        SetupCaptainClaims(1);

        _mockSailingService.Setup(s => s.GetActiveSailByCaptainIdAsync(1))
            .ReturnsAsync((SailWithName?)null);

        var result = await _controller.GetCaptainActiveSail();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetCaptainActiveSail_WhenNoClaimExists_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = await _controller.GetCaptainActiveSail();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetCaptainActiveSail_WhenInvalidClaimValue_ShouldReturnUnauthorized()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, "invalid")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = await _controller.GetCaptainActiveSail();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task AddPositionHistory_WithValidRequest_ShouldReturnCreated()
    {
        SetupCaptainClaims(1);

        var request = new AddPositionRequest(10.5, 20.5, 15.0, 90);

        _mockSailingService.Setup(s => s.AddPositionHistoryAsync(1, It.IsAny<AddPositionRequest>()))
            .ReturnsAsync(true);

        var result = await _controller.AddPositionHistory(request);

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task AddPositionHistory_WhenNoActiveSail_ShouldReturnNotFound()
    {
        SetupCaptainClaims(1);

        var request = new AddPositionRequest(10.5, 20.5, 15.0, 90);

        _mockSailingService.Setup(s => s.AddPositionHistoryAsync(1, It.IsAny<AddPositionRequest>()))
            .ReturnsAsync(false);

        var result = await _controller.AddPositionHistory(request);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = notFoundResult.Value;
        Assert.NotNull(response);
    }

    [Fact]
    public async Task AddPositionHistory_WhenNoClaimExists_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var request = new AddPositionRequest(10.5, 20.5, 15.0, 90);

        var result = await _controller.AddPositionHistory(request);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task CompleteActiveSail_WhenSailCompleted_ShouldReturnOk()
    {
        SetupCaptainClaims(1);

        _mockSailingService.Setup(s => s.CompleteActiveSailAsync(1))
            .ReturnsAsync(true);

        var result = await _controller.CompleteActiveSail();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        Assert.NotNull(response);
    }

    [Fact]
    public async Task CompleteActiveSail_WhenNoActiveSail_ShouldReturnNotFound()
    {
        SetupCaptainClaims(1);

        _mockSailingService.Setup(s => s.CompleteActiveSailAsync(1))
            .ReturnsAsync(false);

        var result = await _controller.CompleteActiveSail();

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = notFoundResult.Value;
        Assert.NotNull(response);
    }

    [Fact]
    public async Task CompleteActiveSail_WhenNoClaimExists_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = await _controller.CompleteActiveSail();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UpdateActiveSailStatus_WithValidStatus_ShouldReturnOk()
    {
        SetupCaptainClaims(1);

        var request = new UpdateSailStatusRequest
        {
            Status = SailStatus.Sailing
        };

        _mockSailingService.Setup(s => s.UpdateSailStatusAsync(1, SailStatus.Sailing))
            .ReturnsAsync(true);

        var result = await _controller.UpdateActiveSailStatus(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        Assert.NotNull(response);
    }

    [Fact]
    public async Task UpdateActiveSailStatus_WhenUpdateFails_ShouldReturnNotFound()
    {
        SetupCaptainClaims(1);

        var request = new UpdateSailStatusRequest
        {
            Status = SailStatus.Finished
        };

        _mockSailingService.Setup(s => s.UpdateSailStatusAsync(1, SailStatus.Finished))
            .ReturnsAsync(false);

        var result = await _controller.UpdateActiveSailStatus(request);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = notFoundResult.Value;
        Assert.NotNull(response);
    }

    [Fact]
    public async Task UpdateActiveSailStatus_WhenNoClaimExists_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var request = new UpdateSailStatusRequest
        {
            Status = SailStatus.Sailing
        };

        var result = await _controller.UpdateActiveSailStatus(request);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UpdateActiveSailStatus_WithMultipleStatusChanges_ShouldCallServiceCorrectly()
    {
        SetupCaptainClaims(1);

        var statuses = new[] { SailStatus.Sailing, SailStatus.Docked, SailStatus.Finished };

        foreach (var status in statuses)
        {
            var request = new UpdateSailStatusRequest { Status = status };

            _mockSailingService.Setup(s => s.UpdateSailStatusAsync(1, status))
                .ReturnsAsync(true);

            var result = await _controller.UpdateActiveSailStatus(request);

            Assert.IsType<OkObjectResult>(result);
        }

        _mockSailingService.Verify(s => s.UpdateSailStatusAsync(1, It.IsAny<SailStatus>()), Times.Exactly(3));
    }

    [Fact]
    public async Task AddPositionHistory_WithMultiplePositions_ShouldCallServiceMultipleTimes()
    {
        SetupCaptainClaims(1);

        var positions = new[]
        {
            new AddPositionRequest(10.0, 20.0, 15.0, 90),
            new AddPositionRequest(11.0, 21.0, 16.0, 95),
            new AddPositionRequest(12.0, 22.0, 17.0, 100)
        };

        _mockSailingService.Setup(s => s.AddPositionHistoryAsync(1, It.IsAny<AddPositionRequest>()))
            .ReturnsAsync(true);

        foreach (var position in positions)
        {
            var result = await _controller.AddPositionHistory(position);
            Assert.IsType<CreatedResult>(result);
        }

        _mockSailingService.Verify(s => s.AddPositionHistoryAsync(1, It.IsAny<AddPositionRequest>()), Times.Exactly(3));
    }
}
