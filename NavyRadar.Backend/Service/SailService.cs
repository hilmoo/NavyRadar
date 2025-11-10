using Npgsql;
using Dapper;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain;
using NavyRadar.Shared.Models;

namespace NavyRadar.Backend.Service;

public class SailService(NpgsqlDataSource dataSource) : ISailService
{
    private const string SelectColumns =
        """
        id AS Id,
        ship_id AS ShipId,
        captain_id AS CaptainId,
        origin_port_id AS OriginPortId,
        destination_port_id AS DestinationPortId,
        status AS Status,
        departure_time AS DepartureTime,
        arrival_time AS ArrivalTime,
        total_distance_nm AS TotalDistanceNm,
        average_speed_knots AS AverageSpeedKnots,
        max_speed_knots AS MaxSpeedKnots
        """;

    public async Task<Sail?> CreateAsync(Sail sail)
    {
        await using var conn = await dataSource.OpenConnectionAsync();

        const string sql =
            $"""
             INSERT INTO "sail" 
                 (ship_id, captain_id, origin_port_id, destination_port_id, status, departure_time, 
                  arrival_time, total_distance_nm, average_speed_knots, max_speed_knots)
             VALUES 
                 (@ShipId, @CaptainId, @OriginPortId, @DestinationPortId, @Status, @DepartureTime, 
                  @ArrivalTime, @TotalDistanceNm, @AverageSpeedKnots, @MaxSpeedKnots)
             RETURNING {SelectColumns};
             """;

        return await conn.QuerySingleOrDefaultAsync<Sail>(sql, sail);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = await dataSource.OpenConnectionAsync();

        const string sql = """DELETE FROM "sail" WHERE id = @Id;""";

        var affectedRows = await conn.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }

    public async Task<IEnumerable<Sail>> GetAllAsync()
    {
        await using var conn = await dataSource.OpenConnectionAsync();

        const string sql = $"""SELECT {SelectColumns} FROM "sail";""";

        return await conn.QueryAsync<Sail>(sql);
    }

    public async Task<IEnumerable<ActiveSailPosition>> GetAllActiveSailPositionAsync()
    {
        const string sql =
            """
            SELECT DISTINCT ON (ph.sail_id)
              ph.sail_id AS SailId,
              s.ship_id AS ShipId,
              sh.name AS ShipName,
              sh.type AS ShipType,
              ph.coordinates AS Coordinates,
              ph.timestamp AS PositionTime
            FROM position_history ph
            JOIN sail s ON s.id = ph.sail_id
            JOIN ship sh ON sh.id = s.ship_id
            WHERE s.status = 'Sailing'
            ORDER BY ph.sail_id, ph.timestamp DESC;
            """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QueryAsync<ActiveSailPosition>(sql);
    }

    public async Task<Sail?> GetByIdAsync(int id)
    {
        await using var conn = await dataSource.OpenConnectionAsync();

        const string sql = $"""SELECT {SelectColumns} FROM "sail" WHERE id = @Id;""";

        return await conn.QuerySingleOrDefaultAsync<Sail>(sql, new { Id = id });
    }

    public async Task<Sail?> UpdateAsync(int id, Sail sail)
    {
        sail.Id = id;

        await using var conn = await dataSource.OpenConnectionAsync();
        const string sql =
            $"""
             UPDATE "sail"
             SET 
                 ship_id = @ShipId,
                 captain_id = @CaptainId,
                 origin_port_id = @OriginPortId,
                 destination_port_id = @DestinationPortId,
                 status = @Status,
                 departure_time = @DepartureTime,
                 arrival_time = @ArrivalTime,
                 total_distance_nm = @TotalDistanceNm,
                 average_speed_knots = @AverageSpeedKnots,
                 max_speed_knots = @MaxSpeedKnots
             WHERE 
                 id = @Id
             RETURNING {SelectColumns};
             """;

        return await conn.QuerySingleOrDefaultAsync<Sail>(sql, sail);
    }
}