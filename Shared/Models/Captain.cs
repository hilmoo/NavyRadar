using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class Captain
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    public string? LicenseNumber { get; set; }
}