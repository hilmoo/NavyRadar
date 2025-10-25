using System.ComponentModel.DataAnnotations;

namespace SharedModels.Models;

public class Captain
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    public string? LicenseNumber { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; }
    public virtual ICollection<Sail> Sails { get; set; } = new List<Sail>();
}