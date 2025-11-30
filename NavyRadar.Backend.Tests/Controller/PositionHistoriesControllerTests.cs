using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.Controller;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Entities;
using NpgsqlTypes;

namespace NavyRadar.Backend.Tests.Controller;

public class PositionHistoriesControllerTests
{
    private readonly Mock<IPositionHistoryService> _mockService;
    private readonly PositionHistoriesController _controller;

    public PositionHistoriesControllerTests()
    {
        _mockService = new Mock<IPositionHistoryService>();
        _controller = new PositionHistoriesController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllPositionHistories_ShouldReturnOkWithPositionHistories()
    {
        var positionHistories = new List<PositionHistory>
        {
            new()
            {
                Id = 1,
                SailId = 1,
                Coordinates = new NpgsqlPoint(10.0, 20.0),
                SpeedKnots = 15.5,
                HeadingDegrees = 180,
                Timestamp = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                SailId = 1,
                Coordinates = new NpgsqlPoint(11.0, 21.0),
                SpeedKnots = 16.0,
                HeadingDegrees = 185,
                Timestamp = DateTime.UtcNow.AddHours(1)
            }
        };

        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(positionHistories);

        var result = await _controller.GetAllPositionHistories();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedHistories = Assert.IsAssignableFrom<IEnumerable<PositionHistory>>(okResult.Value);
        Assert.Equal(2, returnedHistories.Count());
    }

    [Fact]
    public async Task GetPositionHistoryById_WithValidId_ShouldReturnOkWithPosition()
    {
        var position = new PositionHistory
        {
            Id = 1,
            SailId = 1,
            Coordinates = new NpgsqlPoint(10.0, 20.0),
            SpeedKnots = 15.5,
            HeadingDegrees = 180,
            Timestamp = DateTime.UtcNow
        };

        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(position);

        var result = await _controller.GetPositionHistoryById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPosition = Assert.IsType<PositionHistory>(okResult.Value);
        Assert.Equal(1, returnedPosition.Id);
    }

    [Fact]
    public async Task GetPositionHistoryById_WithInvalidId_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((PositionHistory?)null);

        var result = await _controller.GetPositionHistoryById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetPositionHistoriesBySailId_ShouldReturnOkWithPositions()
    {
        var positions = new List<PositionHistory>
        {
            new()
            {
                Id = 1,
                SailId = 1,
                Coordinates = new NpgsqlPoint(10.0, 20.0),
                SpeedKnots = 15.5,
                HeadingDegrees = 180,
                Timestamp = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                SailId = 1,
                Coordinates = new NpgsqlPoint(11.0, 21.0),
                SpeedKnots = 16.0,
                HeadingDegrees = 185,
                Timestamp = DateTime.UtcNow.AddHours(1)
            }
        };

        _mockService.Setup(s => s.GetBySailIdAsync(1)).ReturnsAsync(positions);

        var result = await _controller.GetPositionHistoriesBySailId(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPositions = Assert.IsAssignableFrom<IEnumerable<PositionHistory>>(okResult.Value);
        Assert.Equal(2, returnedPositions.Count());
        Assert.All(returnedPositions, p => Assert.Equal(1, p.SailId));
    }

    [Fact]
    public async Task CreatePositionHistory_WithValidData_ShouldReturnCreatedAtAction()
    {
        var newPosition = new PositionHistory
        {
            SailId = 1,
            Coordinates = new NpgsqlPoint(10.0, 20.0),
            SpeedKnots = 15.5,
            HeadingDegrees = 180,
            Timestamp = DateTime.UtcNow
        };

        var createdPosition = new PositionHistory
        {
            Id = 1,
            SailId = newPosition.SailId,
            Coordinates = newPosition.Coordinates,
            SpeedKnots = newPosition.SpeedKnots,
            HeadingDegrees = newPosition.HeadingDegrees,
            Timestamp = newPosition.Timestamp
        };

        _mockService.Setup(s => s.CreateAsync(It.IsAny<PositionHistory>())).ReturnsAsync(createdPosition);

        var result = await _controller.CreatePositionHistory(newPosition);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedPosition = Assert.IsType<PositionHistory>(createdResult.Value);
        Assert.Equal(1, returnedPosition.Id);
    }

    [Fact]
    public async Task CreatePositionHistory_WhenServiceReturnsNull_ShouldReturnBadRequest()
    {
        var newPosition = new PositionHistory
        {
            SailId = 1,
            Coordinates = new NpgsqlPoint(10.0, 20.0),
            SpeedKnots = 15.5,
            HeadingDegrees = 180,
            Timestamp = DateTime.UtcNow
        };

        _mockService.Setup(s => s.CreateAsync(It.IsAny<PositionHistory>())).ReturnsAsync((PositionHistory?)null);

        var result = await _controller.CreatePositionHistory(newPosition);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Could not create position history.", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdatePositionHistory_WithValidData_ShouldReturnOkWithUpdatedPosition()
    {
        var updatedPosition = new PositionHistory
        {
            Id = 1,
            SailId = 1,
            Coordinates = new NpgsqlPoint(12.0, 22.0),
            SpeedKnots = 17.0,
            HeadingDegrees = 190,
            Timestamp = DateTime.UtcNow
        };

        _mockService.Setup(s => s.UpdateAsync(1, It.IsAny<PositionHistory>())).ReturnsAsync(updatedPosition);

        var result = await _controller.UpdatePositionHistory(1, updatedPosition);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPosition = Assert.IsType<PositionHistory>(okResult.Value);
        Assert.Equal(17.0, returnedPosition.SpeedKnots);
    }

    [Fact]
    public async Task UpdatePositionHistory_WithIdMismatch_ShouldReturnBadRequest()
    {
        var position = new PositionHistory
        {
            Id = 1,
            SailId = 1,
            Coordinates = new NpgsqlPoint(10.0, 20.0),
            SpeedKnots = 15.5,
            HeadingDegrees = 180,
            Timestamp = DateTime.UtcNow
        };

        var result = await _controller.UpdatePositionHistory(2, position);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID mismatch", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdatePositionHistory_WhenNotFound_ShouldReturnNotFound()
    {
        var position = new PositionHistory
        {
            Id = 1,
            SailId = 1,
            Coordinates = new NpgsqlPoint(10.0, 20.0),
            SpeedKnots = 15.5,
            HeadingDegrees = 180,
            Timestamp = DateTime.UtcNow
        };

        _mockService.Setup(s => s.UpdateAsync(1, It.IsAny<PositionHistory>())).ReturnsAsync((PositionHistory?)null);

        var result = await _controller.UpdatePositionHistory(1, position);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeletePositionHistory_WithValidId_ShouldReturnNoContent()
    {
        _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeletePositionHistory(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeletePositionHistory_WhenNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).ReturnsAsync(false);

        var result = await _controller.DeletePositionHistory(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
