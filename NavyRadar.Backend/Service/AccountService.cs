using Npgsql;
using Dapper;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Account;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Service;

public class AccountService(NpgsqlDataSource dataSource) : IAccountService
{
    private const string SelectColumns =
        """
        id AS Id,
        username AS Username,
        password AS Password,
        email AS Email,
        role AS Role
        """;

    public async Task<Account?> CreateAsync(Account account)
    {
        account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);

        const string sql =
            $"""
             INSERT INTO "account" (username, password, email, role)
             VALUES (@Username, @Password, @Email, @Role::account_role)
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Account>(sql, new
        {
            account.Username,
            account.Password,
            account.Email,
            Role = account.Role.ToString()
        });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql =
            """
            DELETE FROM "account" WHERE id = @Id
            """;
        await using var conn = await dataSource.OpenConnectionAsync();

        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "account"
             """;
        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QueryAsync<Account>(sql);
    }

    public async Task<Account?> GetByIdAsync(int id)
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "account" WHERE id = @Id
             """;
        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Account>(sql, new { Id = id });
    }

    public async Task<Account?> UpdateAsync(int id, UpdateAccount account)
    {
        account.Id = id;

        var setClauses = new List<string> { "username = @Username", "email = @Email" };

        if (!string.IsNullOrWhiteSpace(account.Password))
        {
            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            setClauses.Add("password = @Password");
        }

        if (!string.IsNullOrWhiteSpace(account.Role))
        {
            setClauses.Add("role = @Role::account_role");
        }

        var setClause = string.Join(", ", setClauses);

        var sql =
            $"""
             UPDATE "account"
             SET {setClause}
             WHERE id = @Id
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<Account>(sql, new
        {
            account.Id,
            account.Username,
            account.Password,
            account.Email,
            account.Role
        });
    }
}