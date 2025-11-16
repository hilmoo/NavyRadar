using NpgsqlTypes;

namespace NavyRadar.Shared.Domain.Sail;

public class ActiveSailPosition
{
    public int SailId { get; set; }
    public int ShipId { get; set; }
    public required string ShipName { get; set; }
    public required string ShipType { get; set; }
    public NpgsqlPoint Coordinates { get; set; }
    public DateTime PositionTime { get; set; }
}