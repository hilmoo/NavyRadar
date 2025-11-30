using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.Controller;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Sail;
using NavyRadar.Shared.Entities;
using NpgsqlTypes;

namespace NavyRadar.Backend.Tests.Controller;

public class SailsControllerTests
{
    private readonly Mock<ISailService> _mockSailService;
    private readonly SailsController _controller;

    public SailsControllerTests()
    {
        _mockSailService = new Mock<ISailService>();
        _controller = new SailsController(_mockSailService.Object);
    }

    [Fact]
    public async Task GetAllSails_ShouldReturnOkWithSails()
    {
        var sails = new List<SailWithName>
        {
            new() { Id = 1, CaptainId = 1, ShipId = 1, Status = SailStatus.Sailing, DepartureTime = DateTime.UtcNow, OriginPortId = 1, DestinationPortId = 2 },
            new() { Id = 2, CaptainId = 2, ShipId = 2, Status = SailStatus.Docked, DepartureTime = DateTime.UtcNow, OriginPortId = 1, DestinationPortId = 2 }
        };

        _mockSailService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(sails);

        var result = await _controller.GetAllSails();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSails = Assert.IsAssignableFrom<IEnumerable<SailWithName>>(okResult.Value);
        Assert.Equal(2, returnedSails.Count());
    }

    [Fact]
    public async Task GetAllActiveSailPosition_ShouldReturnOkWithActiveSails()
    {
        var activeSails = new List<ActiveSailPosition>
        {
            new() { SailId = 1, ShipId = 1, ShipName = "Ship1", ShipType = "Cargo", Coordinates = new NpgsqlPoint(10.5, 20.5), PositionTime = DateTime.UtcNow }
        };

        _mockSailService.Setup(s => s.GetAllActiveSailPositionAsync())
            .ReturnsAsync(activeSails);

        var result = await _controller.GetAllActiveSailPosition();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSails = Assert.IsAssignableFrom<IEnumerable<ActiveSailPosition>>(okResult.Value);
        Assert.Single(returnedSails);
    }

    [Fact]
    public async Task GetSailById_WhenSailExists_ShouldReturnOkWithSail()
    {
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

        _mockSailService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(sail);

        var result = await _controller.GetSailById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSail = Assert.IsType<SailWithName>(okResult.Value);
        Assert.Equal(1, returnedSail.Id);
    }

    [Fact]
    public async Task GetSailById_WhenSailDoesNotExist_ShouldReturnNotFound()
    {
        _mockSailService.Setup(s => s.GetByIdAsync(999))
            .ReturnsAsync((SailWithName?)null);

        var result = await _controller.GetSailById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateSail_WithValidSail_ShouldReturnCreatedResult()
    {
        var sail = new Sail
        {
            CaptainId = 1,
            ShipId = 1,
            Status = SailStatus.Docked,
            DepartureTime = DateTime.UtcNow,
            OriginPortId = 1,
            DestinationPortId = 2
        };

        var createdSail = new SailWithName
        {
            Id = 1,
            CaptainId = sail.CaptainId,
            ShipId = sail.ShipId,
            Status = sail.Status,
            DepartureTime = sail.DepartureTime,
            OriginPortId = sail.OriginPortId,
            DestinationPortId = sail.DestinationPortId
        };

        _mockSailService.Setup(s => s.CreateAsync(It.IsAny<Sail>()))
            .ReturnsAsync(createdSail);

        var result = await _controller.CreateSail(sail);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(SailsController.GetSailById), createdResult.ActionName);
        var returnedSail = Assert.IsType<SailWithName>(createdResult.Value);
        Assert.Equal(1, returnedSail.Id);
    }

    [Fact]
    public async Task CreateSail_WhenServiceFails_ShouldReturnBadRequest()
    {
        var sail = new Sail
        {
            CaptainId = 1,
            ShipId = 1,
            Status = SailStatus.Docked,
            DepartureTime = DateTime.UtcNow,
            OriginPortId = 1,
            DestinationPortId = 2
        };

        _mockSailService.Setup(s => s.CreateAsync(It.IsAny<Sail>()))
            .ReturnsAsync((SailWithName?)null);

        var result = await _controller.CreateSail(sail);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Could not create sail.", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateSail_WithInvalidModelState_ShouldReturnBadRequest()
    {
        var sail = new Sail { CaptainId = 1, ShipId = 1, Status = SailStatus.Docked, DepartureTime = DateTime.UtcNow, OriginPortId = 1, DestinationPortId = 2 };
        _controller.ModelState.AddModelError("Status", "Status is required");

        var result = await _controller.CreateSail(sail);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateSail_WithValidSail_ShouldReturnOkWithUpdatedSail()
    {
        var sail = new Sail
        {
            Id = 1,
            CaptainId = 1,
            ShipId = 1,
            Status = SailStatus.Sailing,
            DepartureTime = DateTime.UtcNow,
            OriginPortId = 1,
            DestinationPortId = 2
        };

        var updatedSail = new SailWithName
        {
            Id = 1,
            CaptainId = sail.CaptainId,
            ShipId = sail.ShipId,
            Status = sail.Status,
            DepartureTime = sail.DepartureTime,
            OriginPortId = sail.OriginPortId,
            DestinationPortId = sail.DestinationPortId
        };

        _mockSailService.Setup(s => s.UpdateAsync(1, It.IsAny<Sail>()))
            .ReturnsAsync(updatedSail);

        var result = await _controller.UpdateSail(1, sail);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSail = Assert.IsType<SailWithName>(okResult.Value);
        Assert.Equal(1, returnedSail.Id);
    }

    [Fact]
    public async Task UpdateSail_WithIdMismatch_ShouldReturnBadRequest()
    {
        var sail = new Sail { Id = 1, CaptainId = 1, ShipId = 1, Status = SailStatus.Docked, DepartureTime = DateTime.UtcNow, OriginPortId = 1, DestinationPortId = 2 };

        var result = await _controller.UpdateSail(2, sail);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID mismatch", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateSail_WhenSailNotFound_ShouldReturnNotFound()
    {
        var sail = new Sail { Id = 999, CaptainId = 1, ShipId = 1, Status = SailStatus.Docked, DepartureTime = DateTime.UtcNow, OriginPortId = 1, DestinationPortId = 2 };

        _mockSailService.Setup(s => s.UpdateAsync(999, It.IsAny<Sail>()))
            .ReturnsAsync((SailWithName?)null);

        var result = await _controller.UpdateSail(999, sail);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateSail_WithInvalidModelState_ShouldReturnBadRequest()
    {
        var sail = new Sail { Id = 1, CaptainId = 1, ShipId = 1, Status = SailStatus.Docked, DepartureTime = DateTime.UtcNow, OriginPortId = 1, DestinationPortId = 2 };
        _controller.ModelState.AddModelError("Status", "Status is required");

        var result = await _controller.UpdateSail(1, sail);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteSail_WhenSailExists_ShouldReturnNoContent()
    {
        _mockSailService.Setup(s => s.DeleteAsync(1))
            .ReturnsAsync(true);

        var result = await _controller.DeleteSail(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteSail_WhenSailNotFound_ShouldReturnNotFound()
    {
        _mockSailService.Setup(s => s.DeleteAsync(999))
            .ReturnsAsync(false);

        var result = await _controller.DeleteSail(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
