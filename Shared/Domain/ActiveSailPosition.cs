using NpgsqlTypes;

namespace Shared.Domain;

public class ActiveSailPosition
{
    public int SailId { get; set; }
    public int ShipId { get; set; }
    public string ShipName { get; set; }
    public string ShipType { get; set; }
    public NpgsqlPoint Coordinates { get; set; }
    public DateTime PositionTime { get; set; }
}