using System.Text;
using Dapper;
using NavyRadar.Backend.Helper;
using NavyRadar.Backend.IService;
using Npgsql;
using NavyRadar.Shared.Domain.Auth;
using NavyRadar.Shared.Entities;

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

    public async Task<AccountWithAuth?> RegisterAsync(PayloadRegister payloadRegister)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(payloadRegister.Password);

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
                payloadRegister.Username,
                Password = hashedPassword,
                payloadRegister.Email,
                Role = nameof(AccountRole.User)
            });

            if (account == null)
            {
                return null;
            }

            var token = GenerateToken(account);
            return new AccountWithAuth
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

    public async Task<AccountWithAuth?> SignInService(PayloadLogin payloadLogin)
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "account" WHERE username = @Username
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        var account = await conn.QuerySingleOrDefaultAsync<Account>(sql, new { payloadLogin.Username });

        if (account == null)
        {
            return null;
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(payloadLogin.Password, account.Password);

        if (!isPasswordValid)
        {
            return null;
        }

        var token = GenerateToken(account);
        return new AccountWithAuth
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