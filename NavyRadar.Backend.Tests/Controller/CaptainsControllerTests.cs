using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.Controller;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Tests.Controller;

public class CaptainsControllerTests
{
    private readonly Mock<ICaptainService> _mockCaptainService;
    private readonly CaptainsController _controller;

    public CaptainsControllerTests()
    {
        _mockCaptainService = new Mock<ICaptainService>();
        _controller = new CaptainsController(_mockCaptainService.Object);
    }

    [Fact]
    public async Task GetAllCaptains_ShouldReturnOkWithCaptains()
    {
        var captains = new List<Captain>
        {
            new() { Id = 1, AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" },
            new() { Id = 2, AccountId = 2, FirstName = "James", LastName = "Hook", LicenseNumber = "LIC002" }
        };

        _mockCaptainService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(captains);

        var result = await _controller.GetAllCaptains();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCaptains = Assert.IsAssignableFrom<IEnumerable<Captain>>(okResult.Value);
        Assert.Equal(2, returnedCaptains.Count());
    }

    [Fact]
    public async Task GetCaptainById_WhenCaptainExists_ShouldReturnOkWithCaptain()
    {
        var captain = new Captain { Id = 1, AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" };

        _mockCaptainService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(captain);

        var result = await _controller.GetCaptainById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCaptain = Assert.IsType<Captain>(okResult.Value);
        Assert.Equal("Jack", returnedCaptain.FirstName);
        Assert.Equal("Sparrow", returnedCaptain.LastName);
    }

    [Fact]
    public async Task GetCaptainById_WhenCaptainDoesNotExist_ShouldReturnNotFound()
    {
        _mockCaptainService.Setup(s => s.GetByIdAsync(999))
            .ReturnsAsync((Captain?)null);

        var result = await _controller.GetCaptainById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateCaptain_WithValidCaptain_ShouldReturnCreatedResult()
    {
        var captain = new Captain { AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" };
        var createdCaptain = new Captain { Id = 1, AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" };

        _mockCaptainService.Setup(s => s.CreateAsync(It.IsAny<Captain>()))
            .ReturnsAsync(createdCaptain);

        var result = await _controller.CreateCaptain(captain);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CaptainsController.GetCaptainById), createdResult.ActionName);
        var returnedCaptain = Assert.IsType<Captain>(createdResult.Value);
        Assert.Equal(1, returnedCaptain.Id);
    }

    [Fact]
    public async Task CreateCaptain_WhenServiceFails_ShouldReturnBadRequest()
    {
        var captain = new Captain { AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" };

        _mockCaptainService.Setup(s => s.CreateAsync(It.IsAny<Captain>()))
            .ReturnsAsync((Captain?)null);

        var result = await _controller.CreateCaptain(captain);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Could not create captain.", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateCaptain_WithInvalidModelState_ShouldReturnBadRequest()
    {
        var captain = new Captain { AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" };
        _controller.ModelState.AddModelError("FirstName", "FirstName is required");

        var result = await _controller.CreateCaptain(captain);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateCaptain_WithValidCaptain_ShouldReturnOkWithUpdatedCaptain()
    {
        var captain = new Captain { Id = 1, AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" };

        _mockCaptainService.Setup(s => s.UpdateAsync(1, It.IsAny<Captain>()))
            .ReturnsAsync(captain);

        var result = await _controller.UpdateCaptain(1, captain);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCaptain = Assert.IsType<Captain>(okResult.Value);
        Assert.Equal(1, returnedCaptain.Id);
    }

    [Fact]
    public async Task UpdateCaptain_WithIdMismatch_ShouldReturnBadRequest()
    {
        var captain = new Captain { Id = 1, AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" };

        var result = await _controller.UpdateCaptain(2, captain);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID mismatch", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateCaptain_WhenCaptainNotFound_ShouldReturnNotFound()
    {
        var captain = new Captain { Id = 999, AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" };

        _mockCaptainService.Setup(s => s.UpdateAsync(999, It.IsAny<Captain>()))
            .ReturnsAsync((Captain?)null);

        var result = await _controller.UpdateCaptain(999, captain);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateCaptain_WithInvalidModelState_ShouldReturnBadRequest()
    {
        var captain = new Captain { Id = 1, AccountId = 1, FirstName = "Jack", LastName = "Sparrow", LicenseNumber = "LIC001" };
        _controller.ModelState.AddModelError("LicenseNumber", "LicenseNumber is required");

        var result = await _controller.UpdateCaptain(1, captain);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteCaptain_WhenCaptainExists_ShouldReturnNoContent()
    {
        _mockCaptainService.Setup(s => s.DeleteAsync(1))
            .ReturnsAsync(true);

        var result = await _controller.DeleteCaptain(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteCaptain_WhenCaptainNotFound_ShouldReturnNotFound()
    {
        _mockCaptainService.Setup(s => s.DeleteAsync(999))
            .ReturnsAsync(false);

        var result = await _controller.DeleteCaptain(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
