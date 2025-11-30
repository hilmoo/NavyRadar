using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.Controller;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Entities;
using NpgsqlTypes;

namespace NavyRadar.Backend.Tests.Controller;

public class PortsControllerTests
{
    private readonly Mock<IPortService> _mockPortService;
    private readonly PortsController _controller;

    public PortsControllerTests()
    {
        _mockPortService = new Mock<IPortService>();
        _controller = new PortsController(_mockPortService.Object);
    }

    [Fact]
    public async Task GetAllPorts_ShouldReturnOkWithPorts()
    {
        var ports = new List<Port>
        {
            new() { Id = 1, Name = "Port A", CountryCode = "US", Location = new NpgsqlPoint(1.0, 2.0) },
            new() { Id = 2, Name = "Port B", CountryCode = "UK", Location = new NpgsqlPoint(3.0, 4.0) }
        };

        _mockPortService.Setup(s => s.GetAllAsync()).ReturnsAsync(ports);

        var result = await _controller.GetAllPorts();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPorts = Assert.IsAssignableFrom<IEnumerable<Port>>(okResult.Value);
        Assert.Equal(2, returnedPorts.Count());
    }

    [Fact]
    public async Task GetPortById_WithValidId_ShouldReturnOkWithPort()
    {
        var port = new Port
        {
            Id = 1,
            Name = "Test Port",
            CountryCode = "US",
            Location = new NpgsqlPoint(1.0, 2.0)
        };

        _mockPortService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(port);

        var result = await _controller.GetPortById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPort = Assert.IsType<Port>(okResult.Value);
        Assert.Equal(1, returnedPort.Id);
        Assert.Equal("Test Port", returnedPort.Name);
    }

    [Fact]
    public async Task GetPortById_WithInvalidId_ShouldReturnNotFound()
    {
        _mockPortService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Port?)null);

        var result = await _controller.GetPortById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreatePort_WithValidPort_ShouldReturnCreatedAtAction()
    {
        var port = new Port
        {
            Name = "New Port",
            CountryCode = "US",
            Location = new NpgsqlPoint(1.0, 2.0)
        };

        var createdPort = new Port
        {
            Id = 1,
            Name = port.Name,
            CountryCode = port.CountryCode,
            Location = port.Location
        };

        _mockPortService.Setup(s => s.CreateAsync(It.IsAny<Port>())).ReturnsAsync(createdPort);

        var result = await _controller.CreatePort(port);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedPort = Assert.IsType<Port>(createdResult.Value);
        Assert.Equal(1, returnedPort.Id);
        Assert.Equal("New Port", returnedPort.Name);
    }

    [Fact]
    public async Task CreatePort_WhenServiceReturnsNull_ShouldReturnBadRequest()
    {
        var port = new Port
        {
            Name = "New Port",
            CountryCode = "US",
            Location = new NpgsqlPoint(1.0, 2.0)
        };

        _mockPortService.Setup(s => s.CreateAsync(It.IsAny<Port>())).ReturnsAsync((Port?)null);

        var result = await _controller.CreatePort(port);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Could not create port.", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdatePort_WithValidData_ShouldReturnOkWithUpdatedPort()
    {
        var port = new Port
        {
            Id = 1,
            Name = "Updated Port",
            CountryCode = "US",
            Location = new NpgsqlPoint(1.0, 2.0)
        };

        _mockPortService.Setup(s => s.UpdateAsync(1, It.IsAny<Port>())).ReturnsAsync(port);

        var result = await _controller.UpdatePort(1, port);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPort = Assert.IsType<Port>(okResult.Value);
        Assert.Equal("Updated Port", returnedPort.Name);
    }

    [Fact]
    public async Task UpdatePort_WithIdMismatch_ShouldReturnBadRequest()
    {
        var port = new Port
        {
            Id = 1,
            Name = "Test Port",
            CountryCode = "US",
            Location = new NpgsqlPoint(1.0, 2.0)
        };

        var result = await _controller.UpdatePort(2, port);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID mismatch", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdatePort_WhenPortNotFound_ShouldReturnNotFound()
    {
        var port = new Port
        {
            Id = 1,
            Name = "Test Port",
            CountryCode = "US",
            Location = new NpgsqlPoint(1.0, 2.0)
        };

        _mockPortService.Setup(s => s.UpdateAsync(1, It.IsAny<Port>())).ReturnsAsync((Port?)null);

        var result = await _controller.UpdatePort(1, port);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeletePort_WithValidId_ShouldReturnNoContent()
    {
        _mockPortService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeletePort(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeletePort_WhenPortNotFound_ShouldReturnNotFound()
    {
        _mockPortService.Setup(s => s.DeleteAsync(It.IsAny<int>())).ReturnsAsync(false);

        var result = await _controller.DeletePort(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
