using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;
using Dto = NavyRadar.Shared.Spec;

namespace NavyRadar.Shared.Entities;

public class Sail
{
    public int Id { get; set; }

    public int ShipId { get; set; }
    public int CaptainId { get; set; }
    public int OriginPortId { get; set; }
    public int DestinationPortId { get; set; }
    [Required] public required SailStatus Status { get; set; }
    [Required] public required DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public double? TotalDistanceNm { get; set; }
    public double? AverageSpeedKnots { get; set; }
    public double? MaxSpeedKnots { get; set; }
}

public enum SailStatus
{
    [PgName("Docked")] Docked,
    [PgName("Sailing")] Sailing,
    [PgName("Finished")] Finished,
    [PgName("Cancelled")] Cancelled
}

public static class SailMapper
{
    public static Dto.Sail ToDto(this Sail entity)
    {
        return new Dto.Sail
        {
            Id = entity.Id,
            ShipId = entity.ShipId,
            CaptainId = entity.CaptainId,
            OriginPortId = entity.OriginPortId,
            DestinationPortId = entity.DestinationPortId,
            Status = (int)entity.Status,
            DepartureTime = entity.DepartureTime,
            ArrivalTime = entity.ArrivalTime,
            TotalDistanceNm = entity.TotalDistanceNm,
            AverageSpeedKnots = entity.AverageSpeedKnots,
            MaxSpeedKnots = entity.MaxSpeedKnots
        };
    }

    public static Sail ToEntity(this Dto.Sail dto)
    {
        return new Sail
        {
            Id = dto.Id,
            ShipId = dto.ShipId,
            CaptainId = dto.CaptainId,
            OriginPortId = dto.OriginPortId,
            DestinationPortId = dto.DestinationPortId,
            Status = dto.Status switch
            {
                0 => SailStatus.Docked,
                1 => SailStatus.Sailing,
                2 => SailStatus.Finished,
                _ => SailStatus.Cancelled
            },
            DepartureTime = dto.DepartureTime.DateTime,
            ArrivalTime = dto.ArrivalTime?.DateTime,
            TotalDistanceNm = dto.TotalDistanceNm,
            AverageSpeedKnots = dto.AverageSpeedKnots,
            MaxSpeedKnots = dto.MaxSpeedKnots
        };
    }
}