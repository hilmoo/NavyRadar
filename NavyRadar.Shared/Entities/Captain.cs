using System.ComponentModel.DataAnnotations;
using Dto = NavyRadar.Shared.Spec;

namespace NavyRadar.Shared.Entities;

public class Captain
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    [Required] public required string FirstName { get; set; }

    [Required] public required string LastName { get; set; }

    [Required] public required string LicenseNumber { get; set; }
}

public static class CaptainMapper
{
    public static Dto.Captain ToDto(this Captain entity)
    {
        return new Dto.Captain
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            LicenseNumber = entity.LicenseNumber
        };
    }

    public static Captain ToEntity(this Dto.Captain dto)
    {
        return new Captain
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            LicenseNumber = dto.LicenseNumber
        };
    }
}