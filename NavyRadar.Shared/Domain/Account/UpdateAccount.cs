using System.ComponentModel.DataAnnotations;

namespace NavyRadar.Shared.Domain.Account;

public class UpdateAccount
{
    public int Id { get; set; }

    [Required] public required string Username { get; set; }

    public string? Password { get; set; }

    [Required] public required string Email { get; set; }

    public string? Role { get; set; }
}