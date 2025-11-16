using Npgsql;
using Dapper;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Sail;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.Service;

public class SailService(NpgsqlDataSource dataSource) : ISailService
{
    private const string SelectColumns =
        """
        s.id AS Id,
        s.ship_id AS ShipId,
        s.captain_id AS CaptainId,
        s.origin_port_id AS OriginPortId,
        s.destination_port_id AS DestinationPortId,
        s.status AS Status,
        s.departure_time AS DepartureTime,
        s.arrival_time AS ArrivalTime,
        s.total_distance_nm AS TotalDistanceNm,
        s.average_speed_knots AS AverageSpeedKnots,
        s.max_speed_knots AS MaxSpeedKnots
        """;

    public async Task<SailWithName?> CreateAsync(Sail sail)
    {
        await using var conn = await dataSource.OpenConnectionAsync();

        const string sql =
            $"""
             INSERT INTO "sail" 
                 (ship_id, captain_id, origin_port_id, destination_port_id, status, departure_time)
             VALUES 
                 (@ShipId, @CaptainId, @OriginPortId, @DestinationPortId, @Status::sail_status, @DepartureTime)
             RETURNING id;
             """;

        var newId = await conn.QuerySingleAsync<int>(sql, new
        {
            sail.ShipId,
            sail.CaptainId,
            sail.OriginPortId,
            sail.DestinationPortId,
            Status = sail.Status.ToString(),
            sail.DepartureTime
        });

        return await GetByIdAsync(newId);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = await dataSource.OpenConnectionAsync();

        const string sql = """DELETE FROM "sail" WHERE id = @Id;""";

        var affectedRows = await conn.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }

    public async Task<IEnumerable<SailWithName>> GetAllAsync()
    {
        await using var conn = await dataSource.OpenConnectionAsync();

        const string sql = $"""
                            SELECT 
                                {SelectColumns},
                                op.name AS OriginPortName,
                                dp.name AS DestinationPortName
                            FROM "sail" s
                            JOIN "port" op ON s.origin_port_id = op.id
                            JOIN "port" dp ON s.destination_port_id = dp.id;
                            """;

        return await conn.QueryAsync<SailWithName>(sql);
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
            WHERE s.arrival_time IS NULL
            ORDER BY ph.sail_id, ph.timestamp DESC;
            """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QueryAsync<ActiveSailPosition>(sql);
    }

    public async Task<SailWithName?> GetByIdAsync(int id)
    {
        await using var conn = await dataSource.OpenConnectionAsync();

        const string sql = $"""
                            SELECT 
                                {SelectColumns},
                                op.name AS OriginPortName,
                                dp.name AS DestinationPortName
                            FROM "sail" s
                            JOIN "port" op ON s.origin_port_id = op.id
                            JOIN "port" dp ON s.destination_port_id = dp.id
                            WHERE s.id = @Id;
                            """;

        return await conn.QuerySingleOrDefaultAsync<SailWithName>(sql, new { Id = id });
    }

    public async Task<SailWithName?> UpdateAsync(int id, Sail sail)
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
                 status = @Status::sail_status,
                 departure_time = @DepartureTime,
                 arrival_time = @ArrivalTime,
                 total_distance_nm = @TotalDistanceNm,
                 average_speed_knots = @AverageSpeedKnots,
                 max_speed_knots = @MaxSpeedKnots
             WHERE 
                 id = @Id
             RETURNING id;
             """;

        var updatedId = await conn.QuerySingleOrDefaultAsync<int?>(sql, new
        {
            sail.Id,
            sail.ShipId,
            sail.CaptainId,
            sail.OriginPortId,
            sail.DestinationPortId,
            Status = sail.Status.ToString(),
            sail.DepartureTime,
            sail.ArrivalTime,
            sail.TotalDistanceNm,
            sail.AverageSpeedKnots,
            sail.MaxSpeedKnots
        });

        if (updatedId.HasValue)
        {
            return await GetByIdAsync(updatedId.Value);
        }

        return null;
    }
}