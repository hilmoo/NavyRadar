using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NavyRadar.Backend.Controller;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Account;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Tests.Controller;

public class AccountsControllerTests
{
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly AccountsController _controller;

    public AccountsControllerTests()
    {
        _mockAccountService = new Mock<IAccountService>();
        _controller = new AccountsController(_mockAccountService.Object);
    }

    private void SetupUserClaims(int userId, string role = "User")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, userId.ToString()),
            new(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetAllAccounts_ShouldReturnOkWithAccounts()
    {
        var accounts = new List<AccountBase>
        {
            new() { Id = 1, Username = "user1", Email = "user1@test.com", Role = AccountRole.User },
            new() { Id = 2, Username = "user2", Email = "user2@test.com", Role = AccountRole.Admin }
        };

        _mockAccountService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(accounts);

        var result = await _controller.GetAllAccounts();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedAccounts = Assert.IsAssignableFrom<IEnumerable<AccountBase>>(okResult.Value);
        Assert.Equal(2, returnedAccounts.Count());
    }

    [Fact]
    public async Task GetAccountById_WhenAccountExists_ShouldReturnOkWithAccount()
    {
        var account = new AccountBase { Id = 1, Username = "user1", Email = "user1@test.com", Role = AccountRole.User };

        _mockAccountService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(account);

        var result = await _controller.GetAccountById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedAccount = Assert.IsType<AccountBase>(okResult.Value);
        Assert.Equal("user1", returnedAccount.Username);
    }

    [Fact]
    public async Task GetAccountById_WhenAccountDoesNotExist_ShouldReturnNotFound()
    {
        _mockAccountService.Setup(s => s.GetByIdAsync(999))
            .ReturnsAsync((AccountBase?)null);

        var result = await _controller.GetAccountById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateAccount_WithValidAccount_ShouldReturnCreatedResult()
    {
        var accountPassword = new AccountPassword
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "Password123!",
            Role = AccountRole.User
        };

        var createdAccount = new AccountBase
        {
            Id = 1,
            Username = accountPassword.Username,
            Email = accountPassword.Email,
            Role = accountPassword.Role
        };

        _mockAccountService.Setup(s => s.CreateAsync(It.IsAny<AccountPassword>()))
            .ReturnsAsync(createdAccount);

        var result = await _controller.CreateAccount(accountPassword);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(AccountsController.GetAccountById), createdResult.ActionName);
        var returnedAccount = Assert.IsType<AccountBase>(createdResult.Value);
        Assert.Equal(1, returnedAccount.Id);
    }

    [Fact]
    public async Task CreateAccount_WhenServiceFails_ShouldReturnBadRequest()
    {
        var accountPassword = new AccountPassword
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "Password123!",
            Role = AccountRole.User
        };

        _mockAccountService.Setup(s => s.CreateAsync(It.IsAny<AccountPassword>()))
            .ReturnsAsync((AccountBase?)null);

        var result = await _controller.CreateAccount(accountPassword);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Could not create account.", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateAccount_WithInvalidModelState_ShouldReturnBadRequest()
    {
        var accountPassword = new AccountPassword { Username = "newuser", Email = "test@test.com", Password = "Pass123!", Role = AccountRole.User };
        _controller.ModelState.AddModelError("Email", "Email is required");

        var result = await _controller.CreateAccount(accountPassword);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAccount_WithValidAccount_ShouldReturnOkWithUpdatedAccount()
    {
        var updateAccount = new UpdateAccount
        {
            Id = 1,
            Username = "updateduser",
            Email = "updated@test.com"
        };

        var updatedAccount = new AccountBase
        {
            Id = 1,
            Username = updateAccount.Username,
            Email = updateAccount.Email,
            Role = AccountRole.User
        };

        _mockAccountService.Setup(s => s.UpdateAsync(1, It.IsAny<UpdateAccount>()))
            .ReturnsAsync(updatedAccount);

        var result = await _controller.UpdateAccount(1, updateAccount);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedAccount = Assert.IsType<AccountBase>(okResult.Value);
        Assert.Equal("updateduser", returnedAccount.Username);
    }

    [Fact]
    public async Task UpdateAccount_WithIdMismatch_ShouldReturnBadRequest()
    {
        var updateAccount = new UpdateAccount { Id = 1, Username = "user", Email = "user@test.com" };

        var result = await _controller.UpdateAccount(2, updateAccount);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID mismatch", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateAccount_WhenAccountNotFound_ShouldReturnNotFound()
    {
        var updateAccount = new UpdateAccount { Id = 999, Username = "user", Email = "user@test.com" };

        _mockAccountService.Setup(s => s.UpdateAsync(999, It.IsAny<UpdateAccount>()))
            .ReturnsAsync((AccountBase?)null);

        var result = await _controller.UpdateAccount(999, updateAccount);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateAccount_WithInvalidModelState_ShouldReturnBadRequest()
    {
        var updateAccount = new UpdateAccount { Id = 1, Username = "user", Email = "user@test.com" };
        _controller.ModelState.AddModelError("Email", "Email is required");

        var result = await _controller.UpdateAccount(1, updateAccount);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateProfile_WithValidAccount_ShouldReturnOkWithUpdatedAccount()
    {
        SetupUserClaims(1, "User");

        var updateAccount = new UpdateAccount
        {
            Id = 1,
            Username = "updateduser",
            Email = "updated@test.com"
        };

        var updatedAccount = new AccountBase
        {
            Id = 1,
            Username = updateAccount.Username,
            Email = updateAccount.Email,
            Role = AccountRole.User
        };

        _mockAccountService.Setup(s => s.UpdateAsync(1, It.IsAny<UpdateAccount>()))
            .ReturnsAsync(updatedAccount);

        var result = await _controller.UpdateProfile(updateAccount);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedAccount = Assert.IsType<AccountBase>(okResult.Value);
        Assert.Equal("updateduser", returnedAccount.Username);
    }

    [Fact]
    public async Task UpdateProfile_WhenUserTriesToUpdateAnotherProfile_ShouldReturnBadRequest()
    {
        SetupUserClaims(1, "User");

        var updateAccount = new UpdateAccount
        {
            Id = 2,
            Username = "otheruser",
            Email = "other@test.com"
        };

        var result = await _controller.UpdateProfile(updateAccount);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("You can only update your own profile.", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateProfile_WhenNoUserClaimExists_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var updateAccount = new UpdateAccount
        {
            Id = 1,
            Username = "user",
            Email = "user@test.com"
        };

        var result = await _controller.UpdateProfile(updateAccount);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UpdateProfile_WithInvalidUserId_ShouldReturnUnauthorized()
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

        var updateAccount = new UpdateAccount
        {
            Id = 1,
            Username = "user",
            Email = "user@test.com"
        };

        var result = await _controller.UpdateProfile(updateAccount);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid user ID in token", unauthorizedResult.Value);
    }

    [Fact]
    public async Task UpdateProfile_WhenAccountNotFound_ShouldReturnNotFound()
    {
        SetupUserClaims(1, "User");

        var updateAccount = new UpdateAccount
        {
            Id = 1,
            Username = "user",
            Email = "user@test.com"
        };

        _mockAccountService.Setup(s => s.UpdateAsync(1, It.IsAny<UpdateAccount>()))
            .ReturnsAsync((AccountBase?)null);

        var result = await _controller.UpdateProfile(updateAccount);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateProfile_WithInvalidModelState_ShouldReturnBadRequest()
    {
        SetupUserClaims(1, "User");

        var updateAccount = new UpdateAccount { Id = 1, Username = "user", Email = "user@test.com" };
        _controller.ModelState.AddModelError("Email", "Email is required");

        var result = await _controller.UpdateProfile(updateAccount);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAccount_WhenAccountExists_ShouldReturnNoContent()
    {
        _mockAccountService.Setup(s => s.DeleteAsync(1))
            .ReturnsAsync(true);

        var result = await _controller.DeleteAccount(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAccount_WhenAccountNotFound_ShouldReturnNotFound()
    {
        _mockAccountService.Setup(s => s.DeleteAsync(999))
            .ReturnsAsync(false);

        var result = await _controller.DeleteAccount(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
