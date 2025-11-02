using Dapper;
using Backend.IService;
using Npgsql;
using Shared.Models;

namespace Backend.Service;

public class PositionHistoryService(NpgsqlDataSource dataSource) : IPositionHistoryService
{
    private const string SelectColumns =
        """
        id AS Id,
        sail_id AS SailId,
        coordinates AS Coordinates,
        speed_knots AS SpeedKnots,
        heading_degrees AS HeadingDegrees,
        timestamp AS Timestamp
        """;

    public async Task<PositionHistory?> CreateAsync(PositionHistory positionHistory)
    {
        const string sql =
            $"""
             INSERT INTO "position_history" 
                 (sail_id, coordinates, speed_knots, heading_degrees, "timestamp")
             VALUES 
                 (@SailId, @Coordinates, @SpeedKnots, @HeadingDegrees, @Timestamp)
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<PositionHistory>(sql, positionHistory);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql =
            """
            DELETE FROM "position_history" WHERE id = @Id
            """;

        await using var conn = await dataSource.OpenConnectionAsync();

        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<PositionHistory>> GetAllAsync()
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "position_history"
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QueryAsync<PositionHistory>(sql);
    }

    public async Task<PositionHistory?> GetByIdAsync(int id)
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "position_history" WHERE id = @Id
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<PositionHistory>(sql, new { Id = id });
    }

    public async Task<IEnumerable<PositionHistory>> GetBySailIdAsync(int sailId)
    {
        const string sql =
            $"""
             SELECT {SelectColumns} FROM "position_history" WHERE sail_id = @SailId
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QueryAsync<PositionHistory>(sql, new { SailId = sailId });
    }

    public async Task<PositionHistory?> UpdateAsync(int id, PositionHistory positionHistory)
    {
        positionHistory.Id = id;

        const string sql =
            $"""
             UPDATE "position_history"
             SET 
                 sail_id = @SailId,
                 coordinates = @Coordinates,
                 speed_knots = @SpeedKnots,
                 heading_degrees = @HeadingDegrees,
                 "timestamp" = @Timestamp
             WHERE 
                 id = @Id
             RETURNING {SelectColumns}
             """;

        await using var conn = await dataSource.OpenConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<PositionHistory>(sql, positionHistory);
    }
}