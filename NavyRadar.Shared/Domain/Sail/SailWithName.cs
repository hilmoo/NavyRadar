using NavyRadar.Shared.Entities;

namespace NavyRadar.Shared.Domain.Sail;

using Dto = Spec;

public class SailWithName : Entities.Sail
{
    public string? OriginPortName { get; set; }
    public string? DestinationPortName { get; set; }
}

public static class SailWithNameMapper
{
    public static Dto.SailWithName ToDto(this SailWithName entity)
    {
        return new Dto.SailWithName
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
            MaxSpeedKnots = entity.MaxSpeedKnots,
            OriginPortName = entity.OriginPortName,
            DestinationPortName = entity.DestinationPortName
        };
    }

    public static SailWithName ToEntity(this Dto.SailWithName dto)
    {
        return new SailWithName
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
                _ => SailStatus.Cancelled,
            },
            DepartureTime = dto.DepartureTime.DateTime,
            ArrivalTime = dto.ArrivalTime?.DateTime,
            TotalDistanceNm = dto.TotalDistanceNm,
            AverageSpeedKnots = dto.AverageSpeedKnots,
            MaxSpeedKnots = dto.MaxSpeedKnots,
            OriginPortName = dto.OriginPortName,
            DestinationPortName = dto.DestinationPortName
        };
    }
}