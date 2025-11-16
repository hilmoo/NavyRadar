using Dapper;
using NavyRadar.Backend.IService;
using NavyRadar.Shared.Domain.Sail;
using NavyRadar.Shared.Domain.Sailing;
using NavyRadar.Shared.Entities;
using Npgsql;

namespace NavyRadar.Backend.Service;

public class SailingService(NpgsqlDataSource dataSource) : ISailingService
{
    private async Task<int?> _getCaptainIdFromAccountIdAsync(int accountId)
    {
        const string captainIdSql =
            """
            SELECT c.id
            FROM "captain" c
            WHERE c.account_id = @AccountId
            """;
        await using var conn = await dataSource.OpenConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<int?>(captainIdSql, new { AccountId = accountId });
    }

    public async Task<SailWithName?> GetActiveSailByCaptainIdAsync(int accountId)
    {
        var captainId = await _getCaptainIdFromAccountIdAsync(accountId);
        if (captainId == null)
        {
            return null;
        }

        const string sql =
            """
            SELECT 
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
               s.max_speed_knots AS MaxSpeedKnots,
               op.name AS OriginPortName,
               dp.name AS DestinationPortName
            FROM "sail" s
            JOIN "port" op ON s.origin_port_id = op.id
            JOIN "port" dp ON s.destination_port_id = dp.id
            WHERE s.captain_id = @CaptainId
              AND s.arrival_time IS NULL;
            """;

        await using var conn = await dataSource.OpenConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<SailWithName>(sql, new { CaptainId = captainId });
    }

    public async Task<bool> AddPositionHistoryAsync(int accountId, AddPositionRequest request)
    {
        var activeSail = await GetActiveSailByCaptainIdAsync(accountId);
        if (activeSail == null)
        {
            return false;
        }

        const string sql =
            """
            INSERT INTO "position_history" 
                (sail_id, coordinates, speed_knots, heading_degrees)
            VALUES 
                (@SailId, POINT(@LocationX, @LocationY), @SpeedKnots, @HeadingDegrees)
            """;

        await using var conn = await dataSource.OpenConnectionAsync();
        var affectedRows = await conn.ExecuteAsync(sql, new
        {
            SailId = activeSail.Id,
            LocationX = request.Longitude,
            LocationY = request.Latitude,
            request.SpeedKnots,
            request.HeadingDegrees
        });

        return affectedRows > 0;
    }

    public async Task<bool> CompleteActiveSailAsync(int accountId)
    {
        var captainId = await _getCaptainIdFromAccountIdAsync(accountId);
        if (captainId == null)
        {
            return false;
        }

        const string sql =
            """
            -- Find the active sail we need to update
            WITH active_sail AS (
                SELECT id
                FROM "sail"
                WHERE "captain_id" = @CaptainId
                  AND "arrival_time" IS NULL
                LIMIT 1
            ),

            -- Get all position history, using LAG() to get the 'previous' point
            point_data AS (
                SELECT
                    speed_knots,
                    coordinates,
                    LAG(coordinates, 1) OVER (ORDER BY "timestamp") AS prev_coordinates
                FROM "position_history"
                WHERE sail_id = (SELECT id FROM active_sail)
            ),

            -- Calculate all stats in one go
            calculated_stats AS (
                SELECT
                    -- Sum the distances between consecutive points
                    COALESCE(SUM(
                        earth_distance(
                            ll_to_earth(coordinates[1], coordinates[0]),
                            ll_to_earth(prev_coordinates[1], prev_coordinates[0])
                        ) * 0.000539957 -- Convert meters (default) to nautical miles
                    ), 0) AS total_distance_nm,

                    -- Get avg and max speed from the full history
                    (SELECT COALESCE(AVG(speed_knots), 0) FROM point_data) AS avg_speed,
                    (SELECT COALESCE(MAX(speed_knots), 0) FROM point_data) AS max_speed

                FROM point_data
                WHERE prev_coordinates IS NOT NULL
            )

            -- UPDATE the sail table with the calculated values
            UPDATE "sail" AS s
            SET
                "status" = @Status::sail_status,
                "arrival_time" = CURRENT_TIMESTAMP,
                "total_distance_nm" = c.total_distance_nm,
                "average_speed_knots" = c.avg_speed,
                "max_speed_knots" = c.max_speed
            FROM calculated_stats c, active_sail a
            WHERE s.id = a.id
            RETURNING s.id;
            """;

        await using var conn = await dataSource.OpenConnectionAsync();

        var updatedId = await conn.ExecuteScalarAsync<int?>(sql, new
        {
            CaptainId = captainId,
            Status = nameof(SailStatus.Docked)
        });

        return updatedId != null;
    }

    public async Task<bool> UpdateSailStatusAsync(int accountId, SailStatus newStatus)
    {
        if (newStatus != SailStatus.Docked && newStatus != SailStatus.Sailing)
        {
            return false;
        }

        var captainId = await _getCaptainIdFromAccountIdAsync(accountId);
        if (captainId == null)
        {
            return false;
        }

        const string sql =
            """
            UPDATE "sail"
            SET "status" = @Status::sail_status
            WHERE "captain_id" = @CaptainId
              AND "arrival_time" IS NULL;
            """;

        await using var conn = await dataSource.OpenConnectionAsync();
        var affectedRows = await conn.ExecuteAsync(sql, new
        {
            CaptainId = captainId,
            Status = newStatus.ToString()
        });

        return affectedRows > 0;
    }
}