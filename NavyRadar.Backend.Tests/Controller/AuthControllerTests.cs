using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.Controller;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Auth;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Tests.Controller;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [Fact]
    public async Task Register_WithValidPayload_ShouldReturnCreatedResult()
    {
        var payload = new PayloadRegister
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "TestPass123!"
        };

        var accountWithAuth = new AccountWithAuth
        {
            Token = "test-jwt-token",
            UserAccount = new AccountBase
            {
                Id = 1,
                Username = payload.Username,
                Email = payload.Email,
                Role = AccountRole.User
            }
        };

        _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<PayloadRegister>()))
            .ReturnsAsync(accountWithAuth);

        var result = await _controller.Register(payload);

        var createdResult = Assert.IsType<CreatedResult>(result);
        var returnedAccount = Assert.IsType<AccountWithAuth>(createdResult.Value);
        Assert.Equal("test-jwt-token", returnedAccount.Token);
        Assert.Equal(payload.Username, returnedAccount.UserAccount.Username);
    }

    [Fact]
    public async Task Register_WhenServiceReturnsNull_ShouldReturnBadRequest()
    {
        var payload = new PayloadRegister
        {
            Username = "existinguser",
            Email = "existing@example.com",
            Password = "TestPass123!"
        };

        _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<PayloadRegister>()))
            .ReturnsAsync((AccountWithAuth?)null);

        var result = await _controller.Register(payload);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Registration failed. Username or email may already be in use.", badRequestResult.Value);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
    {
        var payload = new PayloadLogin
        {
            Username = "testuser",
            Password = "TestPass123!"
        };

        var accountWithAuth = new AccountWithAuth
        {
            Token = "test-jwt-token",
            UserAccount = new AccountBase
            {
                Id = 1,
                Username = payload.Username,
                Email = "test@example.com",
                Role = AccountRole.User
            }
        };

        _mockAuthService.Setup(s => s.SignInService(It.IsAny<PayloadLogin>()))
            .ReturnsAsync(accountWithAuth);

        var result = await _controller.Login(payload);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedAccount = Assert.IsType<AccountWithAuth>(okResult.Value);
        Assert.Equal("test-jwt-token", returnedAccount.Token);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        var payload = new PayloadLogin
        {
            Username = "testuser",
            Password = "WrongPassword"
        };

        _mockAuthService.Setup(s => s.SignInService(It.IsAny<PayloadLogin>()))
            .ReturnsAsync((AccountWithAuth?)null);

        var result = await _controller.Login(payload);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Register_WithEmptyUsername_ShouldReturnBadRequest()
    {
        var payload = new PayloadRegister
        {
            Username = "",
            Email = "test@example.com",
            Password = "TestPass123!"
        };

        _controller.ModelState.AddModelError("Username", "Username is required");

        var result = await _controller.Register(payload);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
    {
        var payload = new PayloadLogin
        {
            Username = "testuser",
            Password = ""
        };

        _controller.ModelState.AddModelError("Password", "Password is required");

        var result = await _controller.Login(payload);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
