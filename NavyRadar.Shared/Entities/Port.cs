using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;
using Dto = NavyRadar.Shared.Spec;

namespace NavyRadar.Shared.Entities;

public class Port
{
    public int Id { get; set; }

    [Required] public required string Name { get; set; }

    [Required] public required string CountryCode { get; set; }

    [Required] public required NpgsqlPoint Location { get; set; }
}

public static class PortMapper
{
    public static Dto.Port ToDto(this Port entity)
    {
        return new Dto.Port
        {
            Id = entity.Id,
            Name = entity.Name,
            CountryCode = entity.CountryCode,
            Location = new Dto.NpgsqlPoint
                { X = entity.Location.X, Y = entity.Location.Y }
        };
    }

    public static Port ToEntity(this Dto.Port dto)
    {
        return new Port
        {
            Id = dto.Id,
            Name = dto.Name,
            CountryCode = dto.CountryCode,
            Location = new NpgsqlPoint(dto.Location.X, dto.Location.Y)
        };
    }
}