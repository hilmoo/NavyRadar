using Backend.IService;
using Npgsql;
using Dapper;
using Shared.Models;

namespace Backend.Service;

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
             VALUES (@Username, @Password, @Email, @Role)
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Account>(sql, account);
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

    public async Task<Account?> UpdateAsync(int id, Account account)
    {
        account.Id = id;

        string sql;

        if (string.IsNullOrWhiteSpace(account.Password))
        {
            sql =
                $"""
                 UPDATE "account"
                 SET
                     username = @Username,
                     email = @Email,
                     role = @Role
                 WHERE
                     id = @Id
                 RETURNING {SelectColumns}
                 """;
        }
        else
        {
            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            sql =
                $"""
                 UPDATE "account"
                 SET
                     username = @Username,
                     password = @Password,
                     email = @Email,
                     role = @Role
                 WHERE
                     id = @Id
                 RETURNING {SelectColumns}
                 """;
        }

        await using var conn = await dataSource.OpenConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<Account>(sql, account);
    }
}