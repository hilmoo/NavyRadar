using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;
using Dto = NavyRadar.Shared.Spec;

namespace NavyRadar.Shared.Entities;

public class Ship
{
    public int Id { get; set; }
    [Required] public required string ImoNumber { get; set; }
    [Required] public required string MmsiNumber { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required ShipType Type { get; set; }
    [Required] public required int YearBuild { get; set; }
    [Required] public required int LengthOverall { get; set; }
    [Required] public required int GrossTonnage { get; set; }
}

public enum ShipType
{
    [PgName("Cargo Vessels")] [Description("Cargo Vessels")]
    CargoVessels,

    [PgName("Tankers")] Tankers,

    [PgName("Passenger Vessels")] [Description("Passenger Vessels")]
    PassengerVessels,

    [PgName("High Speed Craft")] [Description("High Speed Craft")]
    HighSpeedCraft,

    [PgName("Tugs & Special Craft")] [Description("Tugs & Special Craft")]
    TugsAndSpecialCraft,

    [PgName("Fishing")] Fishing,

    [PgName("Pleasure Craft")] [Description("Pleasure Craft")]
    PleasureCraft,

    [PgName("Navigation Aids")] [Description("Navigation Aids")]
    NavigationAids,

    [PgName("Unspecified Ships")] [Description("Unspecified Ships")]
    UnspecifiedShips
}

public static class ShipMapper
{
    public static Dto.Ship ToDto(this Ship entity)
    {
        return new Dto.Ship
        {
            Id = entity.Id,
            ImoNumber = entity.ImoNumber,
            MmsiNumber = entity.MmsiNumber,
            Name = entity.Name,
            Type = (int)entity.Type,
            YearBuild = entity.YearBuild,
            LengthOverall = entity.LengthOverall,
            GrossTonnage = entity.GrossTonnage
        };
    }

    public static Ship ToEntity(this Dto.Ship dto)
    {
        return new Ship
        {
            Id = dto.Id,
            ImoNumber = dto.ImoNumber,
            MmsiNumber = dto.MmsiNumber,
            Name = dto.Name,
            Type = (ShipType)dto.Type,
            YearBuild = dto.YearBuild,
            LengthOverall = dto.LengthOverall,
            GrossTonnage = dto.GrossTonnage
        };
    }
}