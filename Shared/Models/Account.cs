using System.ComponentModel.DataAnnotations;

namespace SharedModels.Models;

public class Account
{
    public int Id { get; set; }

    [Required] public required string Username { get; set; }

    [Required] public required string Password { get; set; }

    [Required] public required string Email { get; set; }

    [Required] public required string Role { get; set; }

    public virtual Captain? Captain { get; set; }
}