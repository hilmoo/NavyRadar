using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;
using Dto = NavyRadar.Shared.Spec;

namespace NavyRadar.Shared.Entities;

public class Account
{
    public int Id { get; set; }

    [Required] public required string Username { get; set; }

    [Required] public required string Password { get; set; }

    [Required] public required string Email { get; set; }

    [Required] public required AccountRole Role { get; set; }
}

public enum AccountRole
{
    [PgName("User")] User,
    [PgName("Admin")] Admin,
    [PgName("Captain")] Captain
}

public static class AccountMapper
{
    public static Dto.Account ToDto(this Account entity)
    {
        return new Dto.Account
        {
            Id = entity.Id,
            Username = entity.Username,
            Password = entity.Password,
            Email = entity.Email,
            Role = (int)entity.Role
        };
    }

    public static Account ToEntity(this Dto.Account dto)
    {
        return new Account
        {
            Id = dto.Id,
            Username = dto.Username,
            Password = dto.Password,
            Email = dto.Email,
            Role = (AccountRole)dto.Role
        };
    }
}