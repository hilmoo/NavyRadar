using System.ComponentModel.DataAnnotations;

namespace SharedModels.Models;

public class Ship
{
    public int Id { get; set; }

    [Required] public required string ImoNumber { get; set; }

    public string? MmsiNumber { get; set; }

    [Required] public required string Name { get; set; }

    public string? Type { get; set; }
    public int? YearBuild { get; set; }
    public int? LengthOverall { get; set; }
    public int? GrossTonnage { get; set; }

    public virtual ICollection<Sail> Sails { get; set; } = new List<Sail>();
}