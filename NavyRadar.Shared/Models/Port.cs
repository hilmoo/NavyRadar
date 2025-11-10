using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace NavyRadar.Shared.Models;

public class Port
{
    public int Id { get; set; }

    [Required] public required string Name { get; set; }

    [Required] public required string CountryCode { get; set; }

    [Required] public required NpgsqlPoint Location { get; set; }
}