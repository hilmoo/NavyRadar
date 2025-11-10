using System.ComponentModel.DataAnnotations;

namespace NavyRadar.Shared.Domain;

public class UpdateAcc
{
    public int Id { get; set; }

    [Required] public required string Username { get; set; }

    public string? Password { get; set; }

    [Required] public required string Email { get; set; }

    public string? Role { get; set; }
}