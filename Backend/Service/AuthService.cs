using System.Text;
using Backend.IService;
using Backend.lib;
using Dapper;
using Npgsql;
using Shared.Domain;
using Shared.Models;
using Shared.Util;

namespace Backend.Service;

public class AuthService(NpgsqlDataSource dataSource, IConfiguration configuration) : IAuthService
{
    private const string SelectColumns =
        """
        id AS Id,
        username AS Username,
        password AS Password,
        email AS Email,
        role AS Role
        """;

    public async Task<AccWithAuth?> RegisterAsync(RegisterDto registerDto)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        const string sql =
            $"""
             INSERT INTO "account" (username, password, email, role)
             VALUES (@Username, @Password, @Email, @Role)
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        try
        {
            var account = await conn.QuerySingleOrDefaultAsync<Account>(sql, new
            {
                registerDto.Username,
                Password = hashedPassword,
                registerDto.Email,
                Role = nameof(RoleType.User)
            });

            if (account == null)
            {
                return null;
            }

            var token = GenerateToken(account);
            return new AccWithAuth
            {
                Token = token,
                UserAccount = account
            };
        }
        catch (NpgsqlException)
        {
            return null;
        }
    }

    public async Task<AccWithAuth?> SignInService(LoginDto loginDto)
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "account" WHERE username = @Username
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        var account = await conn.QuerySingleOrDefaultAsync<Account>(sql, new { loginDto.Username });

        if (account == null)
        {
            return null;
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, account.Password);

        if (!isPasswordValid)
        {
            return null;
        }

        var token = GenerateToken(account);
        return new AccWithAuth
        {
            Token = token,
            UserAccount = account
        };
    }

    private string GenerateToken(Account account)
    {
        var jwtSecret = configuration["Jwt:Secret"] ?? "secretpasswordsecretpasswordsecretpassword";
        var issuer = configuration["Jwt:Issuer"] ?? "NavyRadar";
        var audience = configuration["Jwt:Audience"] ?? "MyAudience";
        var key = Encoding.UTF8.GetBytes(jwtSecret);

        return Jwt.GenerateJwtToken(account, key, issuer, audience);
    }
}