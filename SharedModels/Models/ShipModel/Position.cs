namespace SharedModels.Models.ShipModel;

public abstract class Position
{
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required DateTime Timestamp { get; set; }
    public double? SpeedOverGround { get; set; }
}
