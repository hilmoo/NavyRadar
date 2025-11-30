using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.Controller;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Tests.Controller;

public class ShipControllerTests
{
    private readonly Mock<IShipService> _mockShipService;
    private readonly ShipController _controller;

    public ShipControllerTests()
    {
        _mockShipService = new Mock<IShipService>();
        _controller = new ShipController(_mockShipService.Object);
    }

    [Fact]
    public async Task GetAllShips_ShouldReturnOkWithShips()
    {
        var ships = new List<Ship>
        {
            new() { Id = 1, Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 },
            new() { Id = 2, Name = "USS Enterprise", Type = ShipType.PassengerVessels, ImoNumber = "IMO7654321", MmsiNumber = "987654321", YearBuild = 1961, LengthOverall = 342, GrossTonnage = 93000 }
        };

        _mockShipService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(ships);

        var result = await _controller.GetAllShips();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedShips = Assert.IsAssignableFrom<IEnumerable<Ship>>(okResult.Value);
        Assert.Equal(2, returnedShips.Count());
    }

    [Fact]
    public async Task GetShipById_WhenShipExists_ShouldReturnOkWithShip()
    {
        var ship = new Ship { Id = 1, Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 };

        _mockShipService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(ship);

        var result = await _controller.GetShipById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedShip = Assert.IsType<Ship>(okResult.Value);
        Assert.Equal("HMS Victory", returnedShip.Name);
    }

    [Fact]
    public async Task GetShipById_WhenShipDoesNotExist_ShouldReturnNotFound()
    {
        _mockShipService.Setup(s => s.GetByIdAsync(999))
            .ReturnsAsync((Ship?)null);

        var result = await _controller.GetShipById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateShip_WithValidShip_ShouldReturnCreatedResult()
    {
        var ship = new Ship { Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 };
        var createdShip = new Ship { Id = 1, Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 };

        _mockShipService.Setup(s => s.CreateAsync(It.IsAny<Ship>()))
            .ReturnsAsync(createdShip);

        var result = await _controller.CreateShip(ship);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ShipController.GetShipById), createdResult.ActionName);
        var returnedShip = Assert.IsType<Ship>(createdResult.Value);
        Assert.Equal(1, returnedShip.Id);
    }

    [Fact]
    public async Task CreateShip_WhenServiceFails_ShouldReturnBadRequest()
    {
        var ship = new Ship { Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 };

        _mockShipService.Setup(s => s.CreateAsync(It.IsAny<Ship>()))
            .ReturnsAsync((Ship?)null);

        var result = await _controller.CreateShip(ship);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Could not create ship.", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateShip_WithInvalidModelState_ShouldReturnBadRequest()
    {
        var ship = new Ship { Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 };
        _controller.ModelState.AddModelError("Name", "Name is required");

        var result = await _controller.CreateShip(ship);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateShip_WithValidShip_ShouldReturnOkWithUpdatedShip()
    {
        var ship = new Ship { Id = 1, Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 };

        _mockShipService.Setup(s => s.UpdateAsync(1, It.IsAny<Ship>()))
            .ReturnsAsync(ship);

        var result = await _controller.UpdateShip(1, ship);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedShip = Assert.IsType<Ship>(okResult.Value);
        Assert.Equal(1, returnedShip.Id);
    }

    [Fact]
    public async Task UpdateShip_WithIdMismatch_ShouldReturnBadRequest()
    {
        var ship = new Ship { Id = 1, Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 };

        var result = await _controller.UpdateShip(2, ship);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID mismatch", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateShip_WhenShipNotFound_ShouldReturnNotFound()
    {
        var ship = new Ship { Id = 999, Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 };

        _mockShipService.Setup(s => s.UpdateAsync(999, It.IsAny<Ship>()))
            .ReturnsAsync((Ship?)null);

        var result = await _controller.UpdateShip(999, ship);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateShip_WithInvalidModelState_ShouldReturnBadRequest()
    {
        var ship = new Ship { Id = 1, Name = "HMS Victory", Type = ShipType.CargoVessels, ImoNumber = "IMO1234567", MmsiNumber = "123456789", YearBuild = 1765, LengthOverall = 69, GrossTonnage = 3500 };
        _controller.ModelState.AddModelError("ImoNumber", "ImoNumber is required");

        var result = await _controller.UpdateShip(1, ship);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteShip_WhenShipExists_ShouldReturnNoContent()
    {
        _mockShipService.Setup(s => s.DeleteAsync(1))
            .ReturnsAsync(true);

        var result = await _controller.DeleteShip(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteShip_WhenShipNotFound_ShouldReturnNotFound()
    {
        _mockShipService.Setup(s => s.DeleteAsync(999))
            .ReturnsAsync(false);

        var result = await _controller.DeleteShip(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
