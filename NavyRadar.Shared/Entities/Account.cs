using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;
using Dto = NavyRadar.Shared.Spec;

namespace NavyRadar.Shared.Entities;

public class AccountBase
{
    public int Id { get; set; }

    [Required] public required string Username { get; set; }
    [Required] public required string Email { get; set; }

    [Required] public required AccountRole Role { get; set; }
}

public class AccountPassword : AccountBase
{
    [Required] public required string Password { get; set; }
}

public enum AccountRole
{
    [PgName("User")] User,
    [PgName("Admin")] Admin,
    [PgName("Captain")] Captain
}

public static class AccountMapper
{
    public static Dto.AccountBase ToDto(this AccountBase entity)
    {
        return new Dto.AccountBase
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            Role = (int)entity.Role
        };
    }

    public static AccountBase ToEntity(this Dto.AccountBase dto)
    {
        return new AccountBase
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            Role = (AccountRole)dto.Role
        };
    }

    public static AccountPassword ToPasswordAccountEntity(this AccountBase baseAccount)
    {
        return new AccountPassword
        {
            Id = baseAccount.Id,
            Username = baseAccount.Username,
            Email = baseAccount.Email,
            Role = baseAccount.Role,
            Password = string.Empty
        };
    }

    public static Dto.AccountPassword ToPasswordAccountDto(this Dto.AccountBase baseAccount)
    {
        return new Dto.AccountPassword
        {
            Id = baseAccount.Id,
            Username = baseAccount.Username,
            Email = baseAccount.Email,
            Role = baseAccount.Role,
            Password = string.Empty
        };
    }
}