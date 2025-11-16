using Npgsql;
using Dapper;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Service;

public class CaptainService(NpgsqlDataSource dataSource) : ICaptainService
{
    private const string SelectColumns =
        """
        id AS Id,
        account_id AS AccountId,
        first_name AS FirstName,
        last_name AS LastName,
        license_number AS LicenseNumber
        """;

    public async Task<Captain?> CreateAsync(Captain captain)
    {
        const string sql =
            $"""
             INSERT INTO "captain" (account_id, first_name, last_name, license_number)
             VALUES (@AccountId, @FirstName, @LastName, @LicenseNumber)
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Captain>(sql, captain);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql =
            """
            DELETE FROM "captain" WHERE id = @Id
            """;
        await using var conn = await dataSource.OpenConnectionAsync();

        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Captain>> GetAllAsync()
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "captain"
             """;
        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QueryAsync<Captain>(sql);
    }

    public async Task<Captain?> GetByIdAsync(int id)
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "captain" WHERE id = @Id
             """;
        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Captain>(sql, new { Id = id });
    }

    public async Task<Captain?> UpdateAsync(int id, Captain captain)
    {
        captain.Id = id;

        const string sql =
            $"""
             UPDATE "captain"
             SET
                 account_id = @AccountId,
                 first_name = @FirstName,
                 last_name = @LastName,
                 license_number = @LicenseNumber
             WHERE
                 id = @Id
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Captain>(sql, captain);
    }
}