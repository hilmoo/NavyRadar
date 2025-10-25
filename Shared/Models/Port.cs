using System.ComponentModel.DataAnnotations;
using SharedModels.Util;

namespace SharedModels.Models;

public class Port
{
    public int Id { get; set; }

    [Required] public required string Name { get; set; }

    [Required] public required string CountryCode { get; set; }

    [Required] public required Point Location { get; set; }
}