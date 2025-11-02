using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace Shared.Models;

public class PositionHistory
{
    public int Id { get; set; }

    public int SailId { get; set; }

    [Required] public required NpgsqlPoint Coordinates { get; set; }

    public decimal? SpeedKnots { get; set; }
    public short? HeadingDegrees { get; set; }

    public DateTime Timestamp { get; set; }
}