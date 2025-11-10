using Npgsql;
using Dapper;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Models;

namespace NavyRadar.Backend.Service;

public class PortService(NpgsqlDataSource dataSource) : IPortService
{
    private const string SelectColumns =
        """
        id AS Id,
        name AS Name,
        country_code AS CountryCode,
        location AS Location
        """;

    public async Task<Port?> CreateAsync(Port port)
    {
        const string sql =
            $"""
             INSERT INTO "port" (name, country_code, location)
             VALUES (@Name, @CountryCode, POINT(@LocationX, @LocationY))
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        var parameters = new
        {
            port.Name,
            port.CountryCode,
            LocationX = port.Location.X,
            LocationY = port.Location.Y
        };

        return await conn.QuerySingleOrDefaultAsync<Port>(sql, parameters);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql =
            """
            DELETE FROM "port" WHERE id = @Id
            """;
        await using var conn = await dataSource.OpenConnectionAsync();

        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Port>> GetAllAsync()
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "port"
             """;
        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QueryAsync<Port>(sql);
    }

    public async Task<Port?> GetByIdAsync(int id)
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "port" WHERE id = @Id
             """;
        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Port>(sql, new { Id = id });
    }

    public async Task<Port?> UpdateAsync(int id, Port port)
    {
        port.Id = id;

        const string sql =
            $"""
             UPDATE "port"
             SET
                 name = @Name,
                 country_code = @CountryCode,
                 location = POINT(@LocationX, @LocationY)
             WHERE 
                 id = @Id
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        var parameters = new
        {
            port.Name,
            port.CountryCode,
            LocationX = port.Location.X,
            LocationY = port.Location.Y,
            port.Id
        };

        return await conn.QuerySingleOrDefaultAsync<Port>(sql, parameters);
    }
}