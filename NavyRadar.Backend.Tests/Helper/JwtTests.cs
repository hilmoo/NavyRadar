using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NavyRadar.Backend.Helper;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Tests.Helper;

public class JwtTests
{
    private readonly byte[] _key;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtTests()
    {
        _key = Encoding.UTF8.GetBytes("ThisIsASecretKeyForTesting123456");
        _issuer = "TestIssuer";
        _audience = "TestAudience";
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidToken()
    {
        var user = new AccountPassword
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            Role = AccountRole.User,
            Password = "hashedPassword"
        };

        var token = Jwt.GenerateJwtToken(user, _key, _issuer, _audience);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateJwtToken_ShouldContainCorrectClaims()
    {
        var user = new AccountPassword
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            Role = AccountRole.Admin,
            Password = "hashedPassword"
        };

        var token = Jwt.GenerateJwtToken(user, _key, _issuer, _audience);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal(user.Username, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal(user.Role.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal(user.Id.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.Sid).Value);
    }

    [Fact]
    public void GenerateJwtToken_ShouldHaveCorrectIssuerAndAudience()
    {
        var user = new AccountPassword
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            Role = AccountRole.User,
            Password = "hashedPassword"
        };

        var token = Jwt.GenerateJwtToken(user, _key, _issuer, _audience);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal(_issuer, jwtToken.Issuer);
        Assert.Contains(_audience, jwtToken.Audiences);
    }

    [Fact]
    public void GenerateJwtToken_ShouldExpireIn120Minutes()
    {
        var user = new AccountPassword
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            Role = AccountRole.User,
            Password = "hashedPassword"
        };

        var beforeGeneration = DateTime.UtcNow;
        var token = Jwt.GenerateJwtToken(user, _key, _issuer, _audience);
        var afterGeneration = DateTime.UtcNow;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expectedExpiration = beforeGeneration.AddMinutes(120);
        var actualExpiration = jwtToken.ValidTo;

        // JWT tokens store timestamps as Unix epoch seconds, which causes subsecond precision loss
        // Allow 1 second tolerance for the expected expiration time
        Assert.True(actualExpiration >= expectedExpiration.AddSeconds(-1),
            $"Token should expire at or after {expectedExpiration.AddSeconds(-1):O}, but expires at {actualExpiration:O}");
        Assert.True(actualExpiration <= afterGeneration.AddMinutes(120).AddSeconds(1),
            $"Token should expire at or before {afterGeneration.AddMinutes(120).AddSeconds(1):O}, but expires at {actualExpiration:O}");
    }

    [Fact]
    public void GenerateJwtToken_ShouldBeValidatableWithSymmetricKey()
    {
        var user = new AccountPassword
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            Role = AccountRole.User,
            Password = "hashedPassword"
        };

        var token = Jwt.GenerateJwtToken(user, _key, _issuer, _audience);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_key),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

        Assert.NotNull(principal);
        Assert.NotNull(validatedToken);
    }
}
