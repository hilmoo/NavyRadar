using System.Text;
using Dapper;
using NavyRadar.Backend.IService;
using NavyRadar.Backend.libs;
using Npgsql;
using NavyRadar.Shared.Domain;
using NavyRadar.Shared.Models;
using NavyRadar.Shared.Util;

namespace NavyRadar.Backend.Service;

public class AuthService(NpgsqlDataSource dataSource) : IAuthService
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

    private static string GenerateToken(Account account)
    {
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")!;
        const string issuer = "NavyRadar";
        const string audience = "NavyRadarUsers";
        var key = Encoding.UTF8.GetBytes(jwtSecret);

        return Jwt.GenerateJwtToken(account, key, issuer, audience);
    }
}