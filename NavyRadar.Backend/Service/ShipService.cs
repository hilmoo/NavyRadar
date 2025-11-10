using Dapper;
using NavyRadar.Backend.IService;
using Npgsql;
using NavyRadar.Shared.Models;


namespace NavyRadar.Backend.Service;

public class ShipService(NpgsqlDataSource dataSource) : IShipService
{
    private const string SelectColumns =
        """
        id AS Id,
        imo_number AS ImoNumber,
        mmsi_number AS MmsiNumber,
        name AS Name,
        type AS Type,
        year_build AS YearBuild,
        length_overall AS LengthOverall,
        gross_tonnage AS GrossTonnage
        """;

    public async Task<IEnumerable<Ship>> GetAllAsync()
    {
        const string sql =
            $"""
              SELECT {SelectColumns} FROM "ship"
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QueryAsync<Ship>(sql);
    }

    public async Task<Ship?> GetByIdAsync(int id)
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "ship" WHERE id = @Id
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Ship>(sql, new { Id = id });
    }

    public async Task<Ship?> CreateAsync(Ship ship)
    {
        const string sql =
            $"""
             INSERT INTO "ship" (imo_number, mmsi_number, name, type, year_build, length_overall, gross_tonnage)
             VALUES (@ImoNumber, @MmsiNumber, @Name, @Type, @YearBuild, @LengthOverall, @GrossTonnage)
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Ship>(sql, ship);
    }

    public async Task<Ship?> UpdateAsync(int id, Ship ship)
    {
        ship.Id = id;

        const string sql =
            $"""
             UPDATE "ship"
             SET imo_number = @ImoNumber,
                 mmsi_number = @MmsiNumber,
                 name = @Name,
                 type = @Type,
                 year_build = @YearBuild,
                 length_overall = @LengthOverall,
                 gross_tonnage = @GrossTonnage
             WHERE id = @Id
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Ship>(sql, ship);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql =
            """
            DELETE FROM "ship" WHERE id = @Id
            """;

        await using var conn = await dataSource.OpenConnectionAsync();

        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}