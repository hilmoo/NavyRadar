using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;

namespace Backend.lib;

public class Jwt
{
    public static string GenerateJwtToken(Account user, byte[] key, string issuer, string audience)
    {
        var securityKey = new SymmetricSecurityKey(key);

        var claims = new Dictionary<string, object>
        {
            [ClaimTypes.Name] = user.Username,
            [ClaimTypes.Email] = user.Email,
            [ClaimTypes.Role] = user.Role,
            [ClaimTypes.Sid] = user.Id
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Claims = claims,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(120),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JsonWebTokenHandler
        {
            SetDefaultTimesOnTokenCreation = false
        };
        var tokenString = handler.CreateToken(descriptor);
        return tokenString;
    }
}