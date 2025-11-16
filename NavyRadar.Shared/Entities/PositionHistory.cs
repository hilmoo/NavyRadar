using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;
using Dto = NavyRadar.Shared.Spec;

namespace NavyRadar.Shared.Entities;

public class PositionHistory
{
    public int Id { get; set; }

    public int SailId { get; set; }

    [Required] public required NpgsqlPoint Coordinates { get; set; }

    public required double SpeedKnots { get; set; }
    public required int HeadingDegrees { get; set; }

    public DateTime Timestamp { get; set; }
}

public static class PositionHistoryMapper
{
    public static Dto.PositionHistory ToDto(this PositionHistory entity)
    {
        return new Dto.PositionHistory
        {
            Id = entity.Id,
            SailId = entity.SailId,
            Coordinates = new Dto.NpgsqlPoint
                { X = entity.Coordinates.X, Y = entity.Coordinates.Y },
            SpeedKnots = entity.SpeedKnots,
            HeadingDegrees = entity.HeadingDegrees,
            Timestamp = entity.Timestamp
        };
    }

    public static PositionHistory ToEntity(this Dto.PositionHistory dto)
    {
        return new PositionHistory
        {
            Id = dto.Id,
            SailId = dto.SailId,
            Coordinates = new NpgsqlPoint(dto.Coordinates.X, dto.Coordinates.Y),
            SpeedKnots = dto.SpeedKnots,
            HeadingDegrees = dto.HeadingDegrees,
            Timestamp = dto.Timestamp.DateTime
        };
    }
}